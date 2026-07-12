using Infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using System.Data.Common;
using Testcontainers.MsSql;

namespace API.test
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithPassword("Str0ng@Passw0rd!")
            .Build();

        private Respawner _respawner = default!;
        private DbConnection _connection = default!;

        // Chamado uma vez pelo xUnit antes dos testes, via IAsyncLifetime.
        // Sobe o container do SQL Server (leva alguns segundos) antes de qualquer teste rodar.
        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        // Implementação explícita para não colidir com o IAsyncDisposable.DisposeAsync
        // que o próprio WebApplicationFactory<T> já implementa.
        async Task IAsyncLifetime.DisposeAsync()
        {
            if (_connection != null)
                await _connection.DisposeAsync();

            await _dbContainer.DisposeAsync();
            await base.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove TODOS os vestígios do registro original do AppDbContext (o que
                // aponta pro SQL Server real via appsettings.json/ConfigMap). RemoveAll
                // (em vez de só Remove/SingleOrDefault) garante que nenhuma configuração
                // antiga fique presa junto da nova, apontando para o banco errado.
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

                // Registra o AppDbContext apontando para o container efêmero do Testcontainers.
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(_dbContainer.GetConnectionString()));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated();
            });
        }

        public async Task ResetDatabaseAsync()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_dbContainer.GetConnectionString());
                await _connection.OpenAsync();
            }

            if (_respawner == null)
            {
                _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
                {
                    TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
                    DbAdapter = DbAdapter.SqlServer
                });
            }

            await _respawner.ResetAsync(_connection);
        }
    }
}
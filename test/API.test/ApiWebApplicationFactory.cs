using Infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
<<<<<<< HEAD:test/API.test/ApiWebApplicationFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
=======
using Microsoft.Data.SqlClient;
>>>>>>> feat/testes-integracao:API.test/ApiWebApplicationFactory.cs
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using System.Data.Common;

namespace API.test
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        private Respawner _respawner = default!;
        private DbConnection _connection = default!;
        private readonly string _connectionString = "Server=localhost,1433;Database=AppDb_IntegrationTests;User Id=sa;Password=Str0ng@Passw0rd!;TrustServerCertificate=True;";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
<<<<<<< HEAD:test/API.test/ApiWebApplicationFactory.cs
            builder.UseEnvironment("IntegrationTests");

            builder.ConfigureServices((context, services) =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

                var connectionString = context.Configuration.GetConnectionString("DefaultTestConnection");

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));

                var sp = services.BuildServiceProvider();

=======
            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();
>>>>>>> feat/testes-integracao:API.test/ApiWebApplicationFactory.cs
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated(); 
            });
        }

        public async Task ResetDatabaseAsync()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                await _connection.OpenAsync();
            }

            if (_respawner == null)
            {
                _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
                {
                    TablesToIgnore = new Table[] { "__EFMigrationsHistory" },
                    DbAdapter = DbAdapter.SqlServer
                });
            }

            await _respawner.ResetAsync(_connection);
        }
    }
}

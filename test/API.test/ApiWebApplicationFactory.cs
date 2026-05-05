using Infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
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
            builder.ConfigureServices(services =>
            {
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
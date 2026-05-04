using Infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API.test
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.RemoveAll(typeof(DbContextOptions<AppDbContext>));

                s.AddDbContext<AppDbContext>(o =>
                    o.UseSqlServer("Server=localhost,1433;Database=AppDb_IntegrationTests;User Id=sa;Password=Str0ng@Passw0rd!;TrustServerCertificate=True;"));

                var sp = s.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }
    }
}

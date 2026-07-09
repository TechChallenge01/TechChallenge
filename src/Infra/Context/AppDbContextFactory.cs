using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Connection string usada apenas pelas ferramentas do EF (migrations, database update).
            // Em runtime a connection string vem do appsettings via DI.
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=AppDb;User Id=sa;Password=Str0ng@Passw0rd!;TrustServerCertificate=True;"
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

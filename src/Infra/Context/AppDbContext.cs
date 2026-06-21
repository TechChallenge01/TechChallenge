using Infra.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Infra.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<UsuarioDbModel> Usuarios { get; set; }
        public DbSet<ClienteDbModel> Clientes { get; set; }
        public DbSet<PecaDbModel> Pecas { get; set; }
        public DbSet<ServicoDbModel> Servicos { get; set; }
        public DbSet<VeiculoDbModel> Veiculos { get; set; }
        public DbSet<OrdemServicoDbModel> OrdensServico { get; set; }
        public DbSet<EstoqueDbModel> Estoques { get; set; }
        public DbSet<EstoqueHistoricoDbmodel> EstoqueHistoricos { get; set; }
        public DbSet<InsumoDbModel> Insumos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Entities;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Infra.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ClienteDbModel> Clientes { get; set; }
        public DbSet<Peca> Pecas { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<VeiculoDbModel> Veiculos { get; set; }
        public DbSet<OrdemServico> OrdensServico { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<EstoqueHistorico> EstoqueHistoricos { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

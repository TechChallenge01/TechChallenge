using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Entities;
using Domain.UnitOfWork;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Infra.Context
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Telefone> Telefones { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Peca> Pecas { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<OrdemServicoPeca> OrdemServicoPecas { get; set; }
        public DbSet<OrdemServicoServico> OrdemServicoServicos { get; set; }
        public DbSet<OrdemServico> OrdensServico { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<EstoqueHistorico> EstoqueHistoricos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await base.SaveChangesAsync(ct);
        }
    }
}

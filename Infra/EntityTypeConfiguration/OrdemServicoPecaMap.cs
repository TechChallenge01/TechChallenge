using Domain.Aggregates.OrdemServicoAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public static class OrdemServicoPecaMap
    {
        public static void ConfigurarOrdemServicoPecas(this EntityTypeBuilder<OrdemServico> builder)
        {
            builder.OwnsMany(os => os.Pecas, osp =>
            {
                osp.ToTable("OrdemServicoPecas");

                osp.WithOwner().HasForeignKey("OrdemServicoId");

                osp.Property<Guid>("Id").ValueGeneratedOnAdd();
                osp.HasKey("Id");

                osp.Property(p => p.PecaId).IsRequired();

                osp.Property(p => p.Quantidade).IsRequired();

                osp.Property(p => p.ValorUnitario)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

                osp.Ignore(p => p.ValorTotal);
                osp.Ignore(p => p.NomePeca);
                osp.Ignore(p => p.DescricaoPeca);
                osp.Ignore(p => p.ValorUnitarioPeca);
            });
        }
    }
}

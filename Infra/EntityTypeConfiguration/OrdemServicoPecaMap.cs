using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class OrdemServicoPecaMap : IEntityTypeConfiguration<OrdemServicoPeca>
    {
        public void Configure(EntityTypeBuilder<OrdemServicoPeca> builder)
        {
            builder.ToTable("OrdemServicoPecas");

            builder.HasKey(osp => new { osp.OrdemServicoId, osp.PecaId });

            builder.Property(osp => osp.OrdemServicoId)
                            .IsRequired();

            builder.Property(osp => osp.PecaId)
                            .IsRequired();

            builder.Property(osp => osp.Quantidade)
                        .IsRequired();

            builder.Property(osp => osp.ValorUnitario)
                            .HasColumnType("decimal(10,2)")
                            .IsRequired();

            builder.HasOne(osp => osp.OrdemServico)
                   .WithMany(os => os.Pecas)
                   .HasForeignKey(osp => osp.PecaId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(osp => osp.ValorTotal);
        }
    }
}

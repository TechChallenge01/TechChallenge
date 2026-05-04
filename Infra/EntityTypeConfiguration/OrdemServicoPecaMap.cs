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

            // Chave composta
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
                   .HasForeignKey(osp => osp.OrdemServicoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(osp => osp.Peca)
                   .WithMany(p => p.OrdemServicoPecas)
                   .HasForeignKey(osp => osp.PecaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(osp => osp.ValorTotal);
        }
    }
}

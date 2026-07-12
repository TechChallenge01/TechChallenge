using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class OrdemServicoInsumoMap : IEntityTypeConfiguration<OrdemServicoInsumoDbModel>
    {
        public void Configure(EntityTypeBuilder<OrdemServicoInsumoDbModel> builder)
        {
            builder.ToTable("OrdemServicoInsumos");

            builder.HasKey(osp => new { osp.OrdemServicoId, osp.InsumoId });

            builder.Property(x => x.Quantidade)
                    .IsRequired();

            builder.Property(x => x.CustoUnitario)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

            builder.HasOne(osi => osi.OrdemServico)
                    .WithMany(os => os.Insumos)
                    .HasForeignKey(osi => osi.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(osi => osi.Insumo)
                   .WithMany(i => i.OrdemServicoInsumos)
                   .HasForeignKey(osi => osi.InsumoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(osi => osi.OrdemServico)
                    .WithMany(os => os.Insumos)
                    .HasForeignKey(osi => osi.OrdemServicoId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(osi => osi.Insumo)
                   .WithMany(i => i.OrdemServicoInsumos)
                   .HasForeignKey(osi => osi.InsumoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

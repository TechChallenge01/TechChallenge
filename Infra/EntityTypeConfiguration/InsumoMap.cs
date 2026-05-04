using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.BaseMap;

namespace Infra.EntityTypeConfiguration
{
    public class InsumoMap : IEntityTypeConfiguration<Insumo>
    {
        public void Configure(EntityTypeBuilder<Insumo> builder)
        {
            builder.ToTable("Insumos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descricao)
                .HasMaxLength(200);

            builder.Property(x => x.CustoUnitario)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.ConfigurarAuditoria();
        }
    }
}

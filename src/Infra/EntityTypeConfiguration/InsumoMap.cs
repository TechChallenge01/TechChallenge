using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class InsumoMap : IEntityTypeConfiguration<InsumoDbModel>
    {
        public void Configure(EntityTypeBuilder<InsumoDbModel> builder)
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

            builder.Property(e => e.IdUsuarioCriacao)
                    .IsRequired();

            builder.Property(e => e.DataCriacao)
                   .IsRequired();

            builder.Property(e => e.IdUsuarioAtualizacao)
                   .IsRequired(false);

            builder.Property(e => e.DataAtualizacao)
                   .IsRequired(false);
        }
    }
}

using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class TelefoneMap : IEntityTypeConfiguration<Telefone>
    {
        public void Configure(EntityTypeBuilder<Telefone> builder)
        {
            builder.ToTable("Telefones");

            builder.Property(t => t.DDD)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(t => t.DDI)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(t => t.Tipo)
                .IsRequired();

            builder.Property(t => t.Numero)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}

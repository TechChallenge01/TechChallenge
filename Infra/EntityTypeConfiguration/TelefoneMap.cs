using Domain.Aggregates.ClienteAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public static class TelefoneMap
    {
        public static void ConfigurarTelefones(this EntityTypeBuilder<Cliente> builder)
        {
            builder.OwnsMany(c => c.Telefones, telefone =>
            {
                telefone.ToTable("ClientesTelefones");

                telefone.WithOwner()
                    .HasForeignKey("ClienteId");

                telefone.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                telefone.HasKey("Id");

                telefone.Property(t => t.DDI)
                    .HasColumnName("DDI")
                    .HasMaxLength(4)
                    .IsRequired();

                telefone.Property(t => t.DDD)
                    .HasColumnName("DDD")
                    .HasMaxLength(3)
                    .IsRequired();

                telefone.Property(t => t.Numero)
                    .HasColumnName("Numero")
                    .HasMaxLength(9)
                    .IsRequired();

                telefone.Property(t => t.Tipo)
                    .HasColumnName("Tipo")
                    .HasMaxLength(20)
                    .IsRequired();
            });
        }
    }
}
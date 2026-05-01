using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class VeiculoMap : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.ToTable("Veiculos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Modelo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MarcaVeiculo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Ano)
                .IsRequired();

            builder.Property(x => x.Placa)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Cor)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteId);
        }
    }
}

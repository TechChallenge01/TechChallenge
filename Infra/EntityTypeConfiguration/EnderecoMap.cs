using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class EnderecoMap : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("Enderecos");

            builder.Property(e => e.Logradouro)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Numero)
                .HasMaxLength(100);

            builder.Property(e => e.Complemento)
                .HasMaxLength(200);

            builder.Property(e => e.Bairro)
                .IsRequired() 
                .HasMaxLength(200);

            builder.Property(e => e.Uf)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(e => e.Cidade)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Cep)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}

using Domain.Aggregates.ClienteAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public static class EnderecoMap
    {
        public static void ConfigurarEnderecos(this EntityTypeBuilder<Cliente> builder)
        {
            builder.OwnsMany(c => c.Enderecos, endereco =>
            {
                endereco.ToTable("ClientesEnderecos");

                endereco.WithOwner()
                        .HasForeignKey("ClienteId");

                endereco.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                endereco.HasKey("Id");

                endereco.Property(e => e.Logradouro)
                        .HasColumnName("Logradouro")
                        .HasMaxLength(200)
                        .IsRequired();

                endereco.Property(e => e.Numero)
                        .HasColumnName("Numero")
                        .HasMaxLength(20)
                        .IsRequired();

                endereco.Property(e => e.Complemento)
                        .HasColumnName("Complemento")
                        .HasMaxLength(100);

                endereco.Property(e => e.Bairro)
                        .HasColumnName("Bairro")
                        .HasMaxLength(100);

                endereco.Property(e => e.Cidade)
                        .HasColumnName("Cidade")
                        .HasMaxLength(100)
                        .IsRequired();

                endereco.Property(e => e.Uf)
                        .HasColumnName("Uf")
                        .HasMaxLength(2)
                        .IsRequired();

                endereco.Property(e => e.Cep)
                        .HasColumnName("Cep")
                        .HasMaxLength(8)
                        .IsRequired();
            });
        }
    }
}
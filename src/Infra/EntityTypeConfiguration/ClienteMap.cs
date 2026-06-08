using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class ClienteMap : IEntityTypeConfiguration<ClienteDbModel>
    {
        public void Configure(EntityTypeBuilder<ClienteDbModel> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Cpf)
                .HasColumnName("Cpf")
                .HasMaxLength(11);

            builder.Property(c => c.Cnpj)
                .HasColumnName("Cnpj")
                .HasMaxLength(14);

            builder.Property(c => c.Logradouro)
                .HasColumnName("Logradouro")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Numero)
                .HasColumnName("Numero")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Complemento)
                .HasColumnName("Complemento")
                .HasMaxLength(200);

            builder.Property(c => c.Bairro)
                .HasColumnName("Bairro")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Cep)
                .HasColumnName("Cep")
                .IsRequired()
                .HasMaxLength(8);

            builder.Property(c => c.Cidade)
                .HasColumnName("Cidade")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Uf)
                .HasColumnName("Uf")
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(c => c.DDD)
                .HasColumnName("DDD")
                .HasMaxLength(3);

            builder.Property(c => c.DDI)
                .HasColumnName("DDI")
                .HasMaxLength(3);

            builder.Property(c => c.NumeroTelefone)
                .HasColumnName("NumeroTelefone")
                .HasMaxLength(9);

            builder.Property(c => c.Email)
                .HasColumnName("Email")
                .HasMaxLength(200);

            builder.Property(e => e.IdUsuarioCriacao)
                   .IsRequired();

            builder.Property(e => e.DataCriacao)
                   .IsRequired();

            builder.Property(e => e.IdUsuarioAtualizacao)
                   .IsRequired(false);

            builder.Property(e => e.DataAtualizacao)
                   .IsRequired(false);

            builder.HasMany(c => c.Veiculos)
                .WithOne(v => v.Cliente)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
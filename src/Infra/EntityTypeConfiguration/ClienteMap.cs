using Domain.Aggregates.ClienteAggregates;
using Infra.BaseMap;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class ClienteMap : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.OwnsOne(c => c.Cpf, cpf =>
            {
                cpf.Property(c => c.Valor).HasColumnName("Cpf").HasMaxLength(11);
            });

            builder.OwnsOne(c => c.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Valor).HasColumnName("Cnpj").HasMaxLength(14);
            });

            builder.OwnsOne(c => c.Endereco, end =>
            {
                end.Property(e => e.Bairro)
                   .HasColumnName("Bairro")
                   .IsRequired()
                   .HasMaxLength(100);

                end.Property(e => e.Logradouro)
                   .HasColumnName("Logradouro")
                   .IsRequired()
                   .HasMaxLength(200);

                end.Property(e => e.Numero)
                    .HasColumnName("Numero")
                    .IsRequired()
                    .HasMaxLength(20);

                end.Property(e => e.Complemento)
                   .HasColumnName("Complemento")
                   .HasMaxLength(200);

                end.Property(e => e.Cep)
                   .HasColumnName("Cep")
                   .IsRequired()
                   .HasMaxLength(8);

                end.Property(e => e.Cidade)
                   .HasColumnName("Cidade")
                   .IsRequired()
                   .HasMaxLength(100);

                end.Property(e => e.Uf)
                   .HasColumnName("Uf")
                   .IsRequired()
                   .HasMaxLength(2);
            });

            builder.OwnsOne(c => c.Telefone, tel =>
            {
                tel.Property(t => t.DDD)
                   .HasColumnName("DDD")
                   .HasMaxLength(3);

                tel.Property(t => t.DDI)
                   .HasColumnName("DDI")
                   .HasMaxLength(3);

                tel.Property(t => t.Numero)
                   .HasMaxLength(9);

            });

            builder.OwnsOne(c => c.Email, em =>
            {
                em.Property(e => e.EnderecoEmail)
                   .HasColumnName("Email")
                   .HasMaxLength(200);
            });

            builder.ConfigurarAuditoria();

            builder.HasMany(c => c.Veiculos)
                .WithOne(v => v.Cliente)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
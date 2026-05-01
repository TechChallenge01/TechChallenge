using Domain.Aggregates.ClienteAggregates;
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

            builder.ConfigurarEmails();
            builder.ConfigurarTelefones();
            builder.ConfigurarEnderecos();

            builder.HasMany(c => c.Veiculos)
                .WithOne()
                .HasForeignKey("ClienteId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
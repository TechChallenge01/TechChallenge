using Domain.Aggregates.ClienteAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public static class EmailMap
    {
        public static void ConfigurarEmails(this EntityTypeBuilder<Cliente> builder)
        {
            builder.OwnsMany(c => c.Emails, email =>
            {
                email.ToTable("ClientesEmails");

                email.WithOwner()
                    .HasForeignKey("ClienteId");

                email.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                email.HasKey("Id");

                email.Property(e => e.EnderecoEmail)
                    .HasColumnName("EnderecoEmail")
                    .HasMaxLength(200)
                    .IsRequired();
            });
        }
    }
}
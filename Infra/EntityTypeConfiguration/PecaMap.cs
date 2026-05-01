using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class PecaMap : IEntityTypeConfiguration<Peca>
    {
        public void Configure(EntityTypeBuilder<Peca> builder)
        {
            builder.ToTable("Pecas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Descricao)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.MarcaPeca)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.ValorUnitario)
                .IsRequired();
        }
    }
}

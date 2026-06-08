using Domain.Aggregates.EstoqueAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration;

public class EstoqueMap : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.ToTable("Estoques");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PecaId);

        builder.Property(e => e.InsumoId);

        builder.HasMany(e => e.Historicos)
               .WithOne(e => e.Estoque)
               .HasForeignKey(e => e.EstoqueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.QuantidadeDisponivel)
                        .IsRequired()
                        .HasDefaultValue(0);

        builder.Property(e => e.QuantidadeReservada)
                        .IsRequired()
                        .HasDefaultValue(0);

        builder.ConfigurarAuditoria();

        builder.Ignore(e => e.QuantidadeTotal);
    }
}

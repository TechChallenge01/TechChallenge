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

        builder.Property(e => e.PecaId)
                        .IsRequired();

        builder.Property(e => e.QuantidadeDisponivel)
                        .IsRequired()
                        .HasDefaultValue(0);

        builder.Property(e => e.QuantidadeReservada)
                        .IsRequired()
                        .HasDefaultValue(0);

        builder.HasOne(e => e.Peca)
                        .WithOne()
                        .HasForeignKey<Estoque>(e => e.PecaId)
                        .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Historicos)
                       .WithOne()
                       .HasForeignKey("EstoqueId")
                       .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.QuantidadeTotal);
        builder.Ignore(e => e.NomePeca);
    }
}

using Domain.Aggregates.EstoqueAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration;

public class EstoqueHistoricoMap : IEntityTypeConfiguration<EstoqueHistorico>
{
    public void Configure(EntityTypeBuilder<EstoqueHistorico> builder)
    {
        builder.ToTable("EstoqueHistoricos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Quantidade)
                        .IsRequired();

        builder.Property(e => e.TipoMovimentacao)
                        .IsRequired()
                        .HasConversion<string>()
                        .HasMaxLength(20);

        builder.Property(e => e.Observacao)
                        .HasMaxLength(500);
    }
}
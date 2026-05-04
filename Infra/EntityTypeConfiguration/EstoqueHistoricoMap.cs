using Domain.Aggregates.EstoqueAggregates;
using Infra.BaseMap;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration;

public class EstoqueHistoricoMap : IEntityTypeConfiguration<EstoqueHistorico>
{
    public void Configure(EntityTypeBuilder<EstoqueHistorico> builder)
    {
        builder.ToTable("EstoqueHistoricos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Quantidade)
                        .IsRequired();

        builder.Property(e => e.TipoMovimentacao)
                        .IsRequired()
                        .HasMaxLength(20);

        builder.Property(e => e.Observacao)
                        .HasMaxLength(500);

        builder.HasOne(x => x.Estoque)
               .WithMany(e => e.Historicos)
               .HasForeignKey(x => x.EstoqueId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigurarAuditoria();
    }
}
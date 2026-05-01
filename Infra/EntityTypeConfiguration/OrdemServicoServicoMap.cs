using Domain.Aggregates.OrdemServicoAggregates;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infra.EntityTypeConfiguration;

public class OrdemServicoServicoMap : IEntityTypeConfiguration<OrdemServicoServico>
{
    public void Configure(EntityTypeBuilder<OrdemServicoServico> builder)
    {
        builder.ToTable("OrdemServicoServicos");

        builder.Property(oss => oss.OrdemServicoId)
                        .IsRequired();

        builder.Property(oss => oss.ServicoId)
                        .IsRequired();

        builder.Property(oss => oss.Quantidade)
                        .IsRequired();

        builder.Property(oss => oss.ValorUnitario)
                        .HasColumnType("decimal(10,2)")
                        .IsRequired();

        builder.Property(oss => oss.DataInicioExecucao);

        builder.Property(oss => oss.DataTerminoExecucao);

        builder.Property(oss => oss.Status)
                        .HasMaxLength(30);

        builder.HasOne(oss => oss.Servico)
                        .WithMany()
                        .HasForeignKey(oss => oss.ServicoId)
                        .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(oss => oss.ValorTotal)
               .Ignore(oss => oss.NomeServico)
               .Ignore(oss => oss.DescricaoServico);
    }
}

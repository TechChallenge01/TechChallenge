using Domain.Aggregates.OrdemServicoAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infra.EntityTypeConfiguration;

public static class OrdemServicoServicoMap
{
    public static void ConfigureOrdemServicoServicos(this EntityTypeBuilder<OrdemServico> builder)
    {
        builder.OwnsMany(os => os.Servicos, oss =>
        {
            oss.ToTable("OrdemServicoServicos");

            oss.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

            oss.HasKey("Id");

            oss.Property(oss => oss.ServicoId)
               .IsRequired();

            oss.Property(oss => oss.Quantidade)
               .IsRequired();

            oss.Property(oss => oss.ValorUnitario)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

            oss.Property(oss => oss.DataInicioExecucao);

            oss.Property(oss => oss.DataTerminoExecucao);

            oss.Property(oss => oss.Status)
               .HasMaxLength(30);

            oss.HasOne(oss => oss.Servico)
               .WithMany()
               .HasForeignKey(oss => oss.ServicoId)
               .OnDelete(DeleteBehavior.Restrict);

            oss.Ignore(oss => oss.ValorTotal)
               .Ignore(oss => oss.NomeServico)
               .Ignore(oss => oss.DescricaoServico);
        });
    }
}

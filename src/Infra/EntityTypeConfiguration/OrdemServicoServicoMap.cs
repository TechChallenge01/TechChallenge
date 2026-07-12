using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infra.EntityTypeConfiguration;

public class OrdemServicoServicoMap : IEntityTypeConfiguration<OrdemServicoServicoDbModel>
{
    public void Configure(EntityTypeBuilder<OrdemServicoServicoDbModel> builder)
    {
        builder.ToTable("OrdemServicoServicos");

        builder.HasKey(oss => new { oss.OrdemServicoId, oss.ServicoId });

        builder.Property(oss => oss.OrdemServicoId)
                        .IsRequired();

        builder.Property(oss => oss.ServicoId)
                        .IsRequired();

        builder.Property(oss => oss.Quantidade)
                        .IsRequired();

        builder.Property(oss => oss.ValorUnitario)
                        .HasColumnType("decimal(10,2)")
                        .IsRequired();

        builder.Property(oss => oss.DataInicioExecucao)
                        .IsRequired(false);

        builder.Property(oss => oss.DataTerminoExecucao)
                        .IsRequired(false);

        builder.Property(oss => oss.Status)
                        .HasConversion<string>()
                        .HasMaxLength(30);

        builder.HasOne(oss => oss.OrdemServico)
               .WithMany(os => os.Servicos)
               .HasForeignKey(oss => oss.OrdemServicoId) 
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oss => oss.Servico)
               .WithMany(s => s.OrdemServicoServicos)
               .HasForeignKey(oss => oss.ServicoId) 
               .OnDelete(DeleteBehavior.Restrict);
    }
}

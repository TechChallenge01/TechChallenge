using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration;

public class OrdemServicoMap : IEntityTypeConfiguration<OrdemServicoDbModel>
{
    public void Configure(EntityTypeBuilder<OrdemServicoDbModel> builder)
    {
        builder.ToTable("OrdemServico");

        builder.HasKey(os => os.Id);

        builder.Property(os => os.VeiculoId)
                        .IsRequired();

        builder.Property(os => os.StatusOS)
                        .IsRequired()
                        .HasConversion<string>()
                        .HasMaxLength(30);

        builder.Property(os => os.Observacao)
                        .HasMaxLength(500);

        builder.Property(os => os.ValorTotal)
                        .HasColumnType("decimal(18,2)")
                        .HasDefaultValue(0);

        builder.Property(os => os.ValorDesconto)
                        .HasColumnType("decimal(18,2)")
                        .HasDefaultValue(0);

        builder.Property(os => os.InicioExecucao)
                .IsRequired(false);

        builder.Property(os => os.TerminoExecucao)
                .IsRequired(false);

        builder.HasOne(os => os.Veiculo)
               .WithMany(s => s.OrdemServicos)
               .HasForeignKey(s => s.VeiculoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(os => os.Cliente)
               .WithMany(c => c.OrdemServicos)
               .HasForeignKey(os => os.ClienteId)
               .OnDelete(DeleteBehavior.Restrict);


        builder.HasMany(os => os.Servicos)
               .WithOne(oss => oss.OrdemServico)
               .HasForeignKey(oss => oss.OrdemServicoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(os => os.Pecas)
               .WithOne(osp => osp.OrdemServico)
               .HasForeignKey(osp => osp.OrdemServicoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(os => os.Insumos)
               .WithOne(osi => osi.OrdemServico)
               .HasForeignKey(osi => osi.OrdemServicoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

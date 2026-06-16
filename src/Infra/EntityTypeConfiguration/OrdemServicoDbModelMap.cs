using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class OrdemServicoDbModelMap : IEntityTypeConfiguration<OrdemServicoDbModel>
    {
        public void Configure(EntityTypeBuilder<OrdemServicoDbModel> builder)
        {
            builder.ToTable("OrdemServico");

            builder.HasKey(os => os.Id);

            builder.Property(os => os.ClienteId)
                .IsRequired();

            builder.Property(os => os.VeiculoId)
                .IsRequired();

            builder.Property(os => os.StatusOS)
                .IsRequired()
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

            builder.Property(os => os.IdUsuarioCriacao)
                .IsRequired();

            builder.Property(os => os.DataCriacao)
                .IsRequired();

            builder.Property(os => os.IdUsuarioAtualizacao)
                .IsRequired(false);

            builder.Property(os => os.DataAtualizacao)
                .IsRequired(false);

            builder.Property(os => os.Ativo)
                .HasDefaultValue(true);
        }
    }
}

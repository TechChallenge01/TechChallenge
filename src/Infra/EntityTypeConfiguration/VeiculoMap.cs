using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class VeiculoMap : IEntityTypeConfiguration<VeiculoDbModel>
    {
        public void Configure(EntityTypeBuilder<VeiculoDbModel> builder)
        {
            builder.ToTable("Veiculos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Modelo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MarcaVeiculo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Ano)
                .IsRequired();

            builder.Property(x => x.Placa)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Cor)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.UsuarioCriacaoId)
                   .IsRequired();

            builder.Property(e => e.DataCriacao)
                   .IsRequired();

            builder.Property(e => e.IdUsuarioAtualizacao)
                   .IsRequired(false);

            builder.Property(e => e.DataAtualizacao)
                   .IsRequired(false);

            builder.HasOne(x => x.Cliente)
                   .WithMany(c => c.Veiculos)
                   .HasForeignKey(x => x.ClienteId);
        }
    }
}

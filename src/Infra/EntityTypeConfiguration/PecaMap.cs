using Domain.Entities;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class PecaMap : IEntityTypeConfiguration<PecaDbModel>
    {
        public void Configure(EntityTypeBuilder<PecaDbModel> builder)
        {
            builder.ToTable("Pecas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Descricao)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.MarcaPeca)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.ValorUnitario)
                .IsRequired();

            builder.Property(e => e.IdUsuarioCriacao)
                   .IsRequired();

            builder.Property(e => e.DataCriacao)
                   .IsRequired();

            builder.Property(e => e.IdUsuarioAtualizacao)
                   .IsRequired(false);

            builder.Property(e => e.DataAtualizacao)
                   .IsRequired(false);
        }
    }
}

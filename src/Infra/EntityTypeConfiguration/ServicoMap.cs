
﻿using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class ServicoMap : IEntityTypeConfiguration<ServicoDbModel>
    {
        public void Configure(EntityTypeBuilder<ServicoDbModel> builder)
        {
            builder.ToTable("Servicos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ValorUnitario)
                .IsRequired();

            builder.Property(x => x.TempoMedioExecucao);

            builder.Property(e => e.IdUsuarioCriacao)
                   .IsRequired();

            builder.Property(e => e.DataCriacao)
                   .IsRequired();

            builder.Property(p => p.ValorUnitario)
                    .HasPrecision(18, 2);

            builder.Property(e => e.IdUsuarioAtualizacao)
                   .IsRequired(false);

            builder.Property(e => e.DataAtualizacao)
                   .IsRequired(false);
        }
    }
}
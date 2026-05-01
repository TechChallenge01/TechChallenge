using Domain.Aggregates.OrdemServicoAggregates;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.EntityTypeConfiguration
{
    public class OrdemServicoPecaMap : IEntityTypeConfiguration<OrdemServicoPeca>
    {
        public void Configure(EntityTypeBuilder<OrdemServicoPeca> builder)
        {
            builder.ToTable("OrdemServicoPecas");

            builder.Property(osp => osp.OrdemServicoId)
                            .IsRequired();

            builder.Property(osp => osp.PecaId)
                            .IsRequired();

            builder.Property(osp => osp.Quantidade)
                        .IsRequired();

            builder.Property(osp => osp.ValorUnitario)
                            .HasColumnType("decimal(10,2)")
                            .IsRequired();

            builder.HasOne(osp => osp.Peca)
                           .WithMany()
                           .HasForeignKey(osp => osp.PecaId)
                           .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(osp => osp.ValorTotal)
                   .Ignore(osp => osp.NomePeca)
                   .Ignore(osp => osp.DescricaoPeca)
                   .Ignore(osp => osp.ValorUnitarioPeca);
        }
    }
}

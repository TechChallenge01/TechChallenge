using Domain.Aggregates.OrdemServicoAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.EntityTypeConfiguration;

public class OrdemServicoMap : IEntityTypeConfiguration<OrdemServico>  
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrdemServico> builder)
    {
        builder.ToTable("OrdemServico");
        
        builder.HasKey(os => os.Id);
       
        builder.Property(os => os.ClienteId)
                        .IsRequired();

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
                        .HasColumnType("datetime")
                        .IsRequired();

        builder.Property(os => os.TerminoExecucao)
                        .HasColumnType("datetime");

        builder.Ignore(os => os.NomeCliente)
               .Ignore(os => os.ModeloVeiculo)
               .Ignore(os => os.PlacaVeiculo)
               .Ignore(os => os.MarcaVeiculo)
               .Ignore(os => os.TempoExecucao);

        builder.HasOne(os => os.Cliente)
                        .WithMany()
                        .HasForeignKey(os => os.ClienteId)
                        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(os => os.Veiculo)
                        .WithMany()
                        .HasForeignKey(os => os.VeiculoId)
                        .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(os => os.Servicos)
                        .WithOne()
                        .HasForeignKey("OsId")
                        .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(os => os.Pecas)
                        .WithOne()
                        .HasForeignKey("OsId")
                        .OnDelete(DeleteBehavior.Cascade);
    }
}

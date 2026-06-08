using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.EntityTypeConfiguration;

public class UsuarioMap : IEntityTypeConfiguration<Usuario>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.SenhaHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Perfil)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.Ativo)
            .HasDefaultValue(true);

        builder.ConfigurarAuditoria();
    }
}
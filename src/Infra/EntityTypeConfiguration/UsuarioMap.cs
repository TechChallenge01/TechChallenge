using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration;

public class UsuarioMap : IEntityTypeConfiguration<UsuarioDbModel>
{
    public void Configure(EntityTypeBuilder<UsuarioDbModel> builder)
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
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
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
=======
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(200);

            builder.Property(x => x.SenhaHash)
                    .IsRequired()
                    .HasMaxLength(64);

            builder.Property(x => x.Perfil)
                    .IsRequired()
                    .HasMaxLength(50);
        }
>>>>>>> 87031395c4d2393cb8f3fe7c2cdeffbe6d3dba83
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
    }
}

using Domain.BaseEntity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityTypeConfiguration
{
    public static class BaseMap
    {
        public static void ConfigurarAuditoria<T>(this EntityTypeBuilder<T> builder)
            where T : Base
        {
            builder.Property(e => e.UsuarioCriacaoId)
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

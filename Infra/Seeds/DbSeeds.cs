using Domain.Entities;
using Domain.Enums;
using Infra.Context;

namespace Infra.Seeds
{
    public static class DbSeeds
    {
        public static async Task Seed(AppDbContext _appDbContext)
        {
            if(!_appDbContext.Usuarios.Any())
            {
                string senha = "12345678";
                Usuario usuario = new Usuario("Admin", "Admin@email.com", BCrypt.Net.BCrypt.HashPassword(senha), EPerfilUsuario.Administrador, Guid.Empty);
                _appDbContext.Usuarios.Add(usuario);
                _appDbContext.SaveChanges();
            }
        }
    }
}

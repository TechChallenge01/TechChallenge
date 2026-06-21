using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Shared.DTOs.Usuarios.Input;

namespace Application.Gateways.Usuarios
{
    public class UsuarioGateway
    {
        private readonly IUsuarioDataSource _usuarioDataSource;

        private UsuarioGateway(IUsuarioDataSource usuarioDataSource)
        {
            _usuarioDataSource = usuarioDataSource;
        }

        public static UsuarioGateway Create(IUsuarioDataSource usuarioDataSource)
        {
            return new UsuarioGateway(usuarioDataSource);
        }

        public async Task<Usuario?> GetByEmail(string Email, CancellationToken ct)
        {
            var usuarioDto = await _usuarioDataSource.GetByEmail(Email, ct);

            Enum.TryParse<EPerfilUsuario>(usuarioDto.Perfil, out var perfilEnum);

            var usuario = new Usuario(usuarioDto.Id, usuarioDto.Nome, usuarioDto.Email, usuarioDto.SenhaHash, usuarioDto.Perfil);

            return usuario;
        }

        public async Task Create(Usuario usuario, CancellationToken ct)
        {
            var usuarioDto = new UsuarioInputDTO
            {
                Id = usuario.Id,
                Email = usuario.Email,
                IdUsuarioCriacao = usuario.IdUsuarioCriacao,
                Nome = usuario.Nome,
                Perfil = usuario.Perfil,
                SenhaHash = usuario.SenhaHash,
                DataAtualizacao = usuario.DataAtualizacao,
                Ativo = usuario.Ativo,
                DataCriacao = usuario.DataCriacao,
                IdUsuarioAtualizacao = usuario.IdUsuarioAtualizacao,
            };

            await _usuarioDataSource.Create(usuarioDto, ct);
        }
    }
}

using Application.Auth.DTOs.Requests;
using Application.Gateways.Usuarios;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Usuarios
{
    public class CriarUsuarioUseCase
    {
        private readonly UsuarioGateway _usuarioGateway;

        public CriarUsuarioUseCase(UsuarioGateway usuarioGateway)
        {
            _usuarioGateway = usuarioGateway;
        }

        public async Task<Guid> Run(CriarUsuarioRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var email = await _usuarioGateway.GetByEmail(request.Email, ct);

                if (email is not null)
                    throw new ArgumentException("Email já cadastrado");

                var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

                var usuario = new Usuario(request.Nome, request.Email, senhaHash, request.Perfil, idUsuario);

                await _usuarioGateway.Create(usuario, ct);

                return usuario.Id;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

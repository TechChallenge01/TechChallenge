using Application.Auth.DTOs.Requests;
using Application.Gateways.Usuarios;
using Application.Interfaces;
using Shared.DTOs.Usuarios.Output;
using Shared.Result;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Application.UseCases.Usuarios
{
    public class LoginUseCase
    {
        private readonly UsuarioGateway _usuarioGateway;
        private readonly IJwtService _jwtService;

        private LoginUseCase(UsuarioGateway usuarioGateway, IJwtService jwtService)
        {
            _usuarioGateway = usuarioGateway;
            _jwtService = jwtService;
        }

        public static LoginUseCase Create(UsuarioGateway usuarioGateway, IJwtService jwtService)
        {
            return new LoginUseCase(usuarioGateway, jwtService);
        }

        public async Task<LoginOutputDTO> Run(LoginRequestDTO request, CancellationToken ct)
        {
            try
            {
                var usuario = await _usuarioGateway.GetByEmail(request.Email, ct);

                if (usuario is null || !usuario.Ativo)
                    throw new UnauthorizedAccessException("Email ou senha Inválidos!");

                var senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash);

                if (!senhaValida)
                    throw new UnauthorizedAccessException("Email ou senha Inválidos!");

                var (token, expiracao) = _jwtService.GerarToken(usuario);

                return new LoginOutputDTO
                {
                    Token = token,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfil = usuario.Perfil.ToString(),
                    Expiracao = expiracao
                };
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

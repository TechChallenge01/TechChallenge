using Application.Auth.DTOs.Requests;
using Application.Auth.DTOs.Responses;
using Shared.Result;
using System.Net;
using Domain.Entities;

namespace Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;
       
        public AuthService(IUsuarioRepository usuarioRepository, IJwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
        }
        public async Task<ICommandResult<Guid>> CriarUsuario(CriarUsuarioRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            try 
            { 
                var existeEmail = await _usuarioRepository.ExisteEmail(request.Email, ct);

                if (existeEmail) 
                    return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = "Este email já está cadastrado." };

                var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

                var usuario = new Usuario(request.Nome, request.Email, senhaHash, request.Perfil, idUsuario);

                await _usuarioRepository.Create(usuario, ct);

                return new CommandResult<Guid> { StatusCode = HttpStatusCode.Created, Message = "Usuário criado com sucesso.", Data = usuario.Id};
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<Guid> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }

        public async Task<ICommandResult<LoginResponseDTO>> Login(LoginRequestDTO request, CancellationToken ct)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByEmail(request.Email, ct);

                if (usuario is null || !usuario.Ativo)
                    return new CommandResult<LoginResponseDTO>
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Message = "Email ou senha inválidos."
                    };

                var senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash);

                if (!senhaValida)
                    return new CommandResult<LoginResponseDTO>
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Message = "Email ou senha inválidos."
                    };

                var (token, expiracao) = _jwtService.GerarToken(usuario);

                return new CommandResult<LoginResponseDTO>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = new LoginResponseDTO
                    {
                        Token = token,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Perfil = usuario.Perfil.ToString(),
                        Expiracao = expiracao
                    },
                    Message = "Login realizado com sucesso."
                };
            }
            catch (ArgumentException ex)
            {
                return new CommandResult<LoginResponseDTO> { StatusCode = HttpStatusCode.BadRequest, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new CommandResult<LoginResponseDTO> { StatusCode = HttpStatusCode.InternalServerError, Message = $"Erro interno no servidor. Detalhes: {ex.Message}" };
            }
        }
    }
}

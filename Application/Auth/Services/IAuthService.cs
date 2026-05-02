using Application.Auth.DTOs.Requests;
using Application.Auth.DTOs.Responses;
using Shared.Result;

namespace Application.Auth.Services;

public interface IAuthService
{
    Task<ICommandResult<LoginResponseDTO>> Login(LoginRequestDTO request, CancellationToken ct);
    Task<ICommandResult<Guid>> CriarUsuario(CriarUsuarioRequestDTO request, CancellationToken ct);
}

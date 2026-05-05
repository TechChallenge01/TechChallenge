using Application.Pecas.DTOs.Requests;
using Application.Pecas.DTOs.Responses;
using Shared.DTOs;
using Shared.Result;

namespace Application.Pecas.Services
{
    public interface IPecaService
    {
        Task<ICommandResult<PagedResultDTO<PecaResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<Guid>> Create(PecaRequestDTO request, Guid idUsuario, CancellationToken ct);
        Task<ICommandResult> Delete(Guid id, Guid idUsuario, CancellationToken ct);
        Task<ICommandResult> Update(Guid id, Guid idUsuario, PecaRequestDTO request, CancellationToken ct);
        Task<ICommandResult<PecaResponseDTO>> GetById(Guid id, CancellationToken ct);
    }
}

using Application.Estoques.DTOs.Requests;
using Application.Estoques.DTOs.Responses;
using Shared.Result;
using Shared.Result.DTO;

namespace Application.Estoques.Services
{
    public interface IEstoqueService
    {
        Task<ICommandResult<PagedResultDTO<EstoqueResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<EstoqueResponseDTO>> GetById(Guid id, CancellationToken ct);
        Task<ICommandResult<Guid>> Movimetar(EstoqueRequestDTO request, CancellationToken ct);
    }
}

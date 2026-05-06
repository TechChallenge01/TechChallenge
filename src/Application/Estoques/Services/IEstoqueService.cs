using Application.Estoques.DTOs.Requests;
using Application.Estoques.DTOs.Responses;
using Shared.DTOs;
using Shared.Result;

namespace Application.Estoques.Services
{
    public interface IEstoqueService
    {
        Task<ICommandResult<PagedResultDTO<EstoqueResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<EstoqueByIdResponseDTO>> GetById(Guid id, CancellationToken ct);
        Task<ICommandResult<Guid>> Movimetar(EstoqueRequestDTO request,Guid idUsuario, CancellationToken ct);
    }
}

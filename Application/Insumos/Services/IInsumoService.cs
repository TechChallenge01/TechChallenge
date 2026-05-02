using Application.Insumos.DTOs.Requests;
using Application.Insumos.DTOs.Responses;
using Shared.DTOs;
using Shared.Result;

namespace Application.Insumos.Services
{
    public interface IInsumoService
    {
        Task<ICommandResult<PagedResultDTO<InsumoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken cancellationToken);
        Task<ICommandResult<InsumoResponseDTO>> GetById(Guid id, CancellationToken cancellationToken);
        Task<ICommandResult<Guid>> Create(InsumoRequestDTO request, Guid idUsuario, CancellationToken cancellationToken);
        Task<ICommandResult> Update(Guid id, Guid idUsuario, InsumoRequestDTO request, CancellationToken cancellationToken);
        Task<ICommandResult> Delete(Guid id, Guid idUsuario, CancellationToken cancellationToken);
    }
}

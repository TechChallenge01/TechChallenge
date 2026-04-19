using Application.Cliente.DTOs.Requests;
using Application.Cliente.DTOs.Responses;
using Shared.Result;
using Shared.Result.DTO;

namespace Application.Cliente.Services
{
    public interface IClienteService
    {
        Task<ICommandResult<PagedResultDTO<ClienteResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<Guid>> Create(ClienteRequestDTO request, CancellationToken ct);
        Task<ICommandResult> Delete(Guid id, CancellationToken ct);
        Task<ICommandResult> Update(Guid id, ClienteRequestDTO request, CancellationToken ct);
    }
}

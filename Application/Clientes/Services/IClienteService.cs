using Application.Clientes.DTOs.Requests;
using Application.Clientes.DTOs.Responses;
using Shared.DTOs;
using Shared.Result;

namespace Application.Clientes.Services
{
    public interface IClienteService
    {
        Task<ICommandResult<PagedResultDTO<ClienteResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<Guid>> Create(ClienteRequestDTO request, CancellationToken ct);
        Task<ICommandResult> Delete(Guid id, CancellationToken ct);
        Task<ICommandResult> Update(Guid id, ClienteRequestDTO request, CancellationToken ct);
        Task<ICommandResult<ClienteResponseDTO>> GetById(Guid id, CancellationToken ct);
    }
}

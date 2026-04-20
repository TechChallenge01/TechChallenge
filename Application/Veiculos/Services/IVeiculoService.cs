using Application.Veiculos.DTOs.Requests;
using Application.Veiculos.DTOs.Response;
using Shared.Result;
using Shared.Result.DTO;

namespace Application.Veiculos.Services
{
    public interface IVeiculoService
    {
        Task<ICommandResult<PagedResultDTO<VeiculoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<Guid>> Create(VeiculoRequestDTO request, CancellationToken ct);
        Task<ICommandResult> Delete(Guid Id, CancellationToken ct);
        Task<ICommandResult> Update(Guid Id, VeiculoRequestDTO request, CancellationToken ct);
    }
}

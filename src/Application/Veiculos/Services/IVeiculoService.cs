using Application.Veiculos.DTOs.Requests;
using Application.Veiculos.DTOs.Response;
using Shared.DTOs;
using Shared.Result;

namespace Application.Veiculos.Services
{
    public interface IVeiculoService
    {
        Task<ICommandResult<PagedResultDTO<VeiculoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<Guid>> Create(VeiculoRequestDTO request, Guid idUsuario, CancellationToken ct);
        Task<ICommandResult> Delete(Guid Id, Guid idUsuario, CancellationToken ct);
        Task<ICommandResult> Update(Guid Id, Guid idUsuario, VeiculoRequestDTO request, CancellationToken ct);
        Task<ICommandResult<VeiculoResponseDTO>> GetById(Guid Id, CancellationToken ct);
    }
}

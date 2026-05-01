using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.DTOs.Responses;
using Shared.Result;
using Shared.Result.DTO;

namespace Application.OrdemServicos.Services;

public interface IOrdemServicoService
{
    Task<ICommandResult<PagedResultDTO<OrdemServicoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
    Task<ICommandResult<Guid>> Create(OrdemServicoRequestDTO request, CancellationToken ct);
    Task<ICommandResult> Cancelar(Guid id, CancellationToken ct);
    Task<ICommandResult> Aprovar(Guid id, CancellationToken ct);
    Task<ICommandResult> FinalizarServico(Guid id, FinalizarServicoDTO dto, CancellationToken ct);
    Task<ICommandResult<OrdemServicoResponseDTO>> GetById(Guid id, CancellationToken ct);
    Task<ICommandResult> RealizarDiagnostico(Guid id, DiagnosticoRequestDTO request, CancellationToken ct);
}

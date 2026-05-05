using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.DTOs.Responses;
using Shared.DTOs;
using Shared.Result;

namespace Application.OrdemServicos.Services;

public interface IOrdemServicoService
{
    Task<ICommandResult<PagedResultDTO<OrdemServicoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
    Task<ICommandResult<Guid>> Create(OrdemServicoRequestDTO request, Guid idUsuario, CancellationToken ct);
    Task<ICommandResult> Cancelar(Guid id, Guid idUsuario, CancellationToken ct);
    Task<ICommandResult> Aprovar(Guid id, Guid idUsuario, CancellationToken ct);
    Task<ICommandResult> FinalizarServico(Guid id, Guid idUsuario, FinalizarServicoDTO dto, CancellationToken ct);
    Task<ICommandResult<OrdemServicoResponseDTO>> GetById(Guid id, CancellationToken ct);
    Task<ICommandResult> IniciarDiagnostico(Guid id, Guid idUsuario, CancellationToken ct);
    Task<ICommandResult> RealizarDiagnostico(Guid id, Guid idUsuario, DiagnosticoRequestDTO request, CancellationToken ct);
    Task<ICommandResult> RegistrarEntrega(Guid id, Guid idUsuario, CancellationToken ct);
}

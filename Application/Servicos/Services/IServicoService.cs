using Application.OrdemServicos.DTOs.Requests;
using Application.Servicos.DTOs.Requests;
using Application.Servicos.DTOs.Response;
using Shared.DTO;
using Shared.Result;

namespace Application.Servicos.Services;

public interface IServicoService
{
    Task<ICommandResult<PagedResultDTO<ServicoResponseDTO>>> GetPaginated (int page, int pageSize, CancellationToken ct);
    Task<ICommandResult<Guid>> Create(ServicoRequestDTO request, CancellationToken ct);
    Task<ICommandResult> Delete(Guid Id, CancellationToken ct);
    Task<ICommandResult> Update(Guid Id, ServicoRequestDTO request, CancellationToken ct);
    Task<ICommandResult<ServicoResponseDTO>> GetById(Guid Id, CancellationToken ct);
}

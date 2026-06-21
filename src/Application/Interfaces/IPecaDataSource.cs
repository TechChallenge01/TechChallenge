using Shared.DTOs.Pecas.Input;

namespace Application.Interfaces;

public interface IPecaDataSource
{
    Task<(List<PecaInputDTO> pecas, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
    Task<PecaInputDTO?> GetById(Guid id, CancellationToken ct);
    Task<List<PecaInputDTO>?> GetByIds(List<Guid> id, CancellationToken ct);
    Task Create(PecaInputDTO request, CancellationToken ct);
    Task Update(PecaInputDTO request, CancellationToken ct);
}
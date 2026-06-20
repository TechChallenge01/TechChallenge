using Shared.DTOs.Insumos.Input;

namespace Application.Interfaces;

public interface IInsumoDataSource
{
    Task<(List<InsumoInputDTO> insumos, int total)> GetPaginated(int page, int pageSize, CancellationToken cancellationToken);
    Task<InsumoInputDTO> GetById(Guid id, CancellationToken cancellationToken);
    Task<List<InsumoInputDTO>> GetByIds(List<Guid> id, CancellationToken cancellationToken);
    Task Create(InsumoInputDTO request, CancellationToken cancellationToken);
    Task Update(InsumoInputDTO request, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}

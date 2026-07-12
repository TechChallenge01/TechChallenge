using Shared.DTOs.Estoques.Input;

namespace Application.Interfaces;

public interface IEstoqueDataSource
{
    Task<(List<EstoqueInputDTO> estoques, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
    Task<EstoqueInputDTO?> GetById(Guid id, CancellationToken ct);
    Task<EstoqueInputDTO?> GetByInsumoId(Guid insumoId, CancellationToken ct);
    Task<EstoqueInputDTO?> GetByPecaId(Guid pecaId, CancellationToken ct);
    Task<List<EstoqueInputDTO>?> GetByPecasIds(ICollection<Guid> ids, CancellationToken ct);
    Task<List<EstoqueInputDTO>?> GetByInsumosIds(ICollection<Guid> ids, CancellationToken ct);
    Task Update(EstoqueInputDTO estoque, CancellationToken ct);
    Task UpdateEstoques(ICollection<EstoqueInputDTO> estoque, CancellationToken ct);
}

namespace Domain.Entities.Repositories
{
    public interface IInsumoRepository
    {
        Task<(ICollection<Insumo> insumos, int total)> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<Insumo?> GetById(Guid id, CancellationToken cancellationToken);
        Task<ICollection<Insumo>> GetByIds (ICollection<Guid> ids, CancellationToken cancellationToken);
        Task Create(Insumo insumo, CancellationToken ct);
        Task Delete(Insumo insumo, CancellationToken ct);
    }
}

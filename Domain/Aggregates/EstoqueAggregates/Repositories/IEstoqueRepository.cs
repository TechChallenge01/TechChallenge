namespace Domain.Aggregates.EstoqueAggregates.Repositories
{
    public interface IEstoqueRepository
    {
        Task<(ICollection<Estoque> estoques, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<Estoque> GetByPecaId(Guid Peca, CancellationToken ct);
        Task<Estoque> GetByInsumoId(Guid Insumo, CancellationToken ct);
        Task<ICollection<Estoque>> GetByPecaIds(ICollection<Guid> Pecas, CancellationToken ct);
        Task<ICollection<Estoque>> GetByInsumoIds(ICollection<Guid> Insumos, CancellationToken ct);
        Task<Estoque> GetById(Guid id, CancellationToken ct);
        Task Create(Estoque estoque, CancellationToken ct);
        Task<Estoque> GetByInsumoId(Guid Insumo, CancellationToken ct);
    }
}

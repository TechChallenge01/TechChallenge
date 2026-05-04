namespace Domain.Entities.Repositories
{
    public interface IPecaRepository
    {
        Task<(ICollection<Peca> pecas, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<Peca?> GetById(Guid id, CancellationToken ct);
        Task Create(Peca peca, CancellationToken ct);
        Task Delete(Peca peca, CancellationToken ct);
        Task<List<Peca>> GetByIds(List<Guid> idsPecas, CancellationToken ct);
    }
}

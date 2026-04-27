namespace Domain.Entities.Repositories;

public interface IServicoRepository
{
    Task<Servico> GetById(Guid id, CancellationToken ct);
    Task<(List<Servico> servicos, int total)> GetPaginatedList(int page, int pageSize, CancellationToken ct);
    Task Create(Servico servico, CancellationToken ct);
    Task Delete(Servico servico, CancellationToken ct);
    Task<ICollection<Servico>> GetByIds(ICollection<Guid> idsServicos, CancellationToken ct);
}

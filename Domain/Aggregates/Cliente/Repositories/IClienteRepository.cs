using Domain.Agregates.Cliente;

namespace Domain.Aggregates.Cliente.Repositories
{
    public interface IClienteRepository
    {
        Task<(ICollection<ClienteEntity> Clientes, int Total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task Create(ClienteEntity entity, CancellationToken ct);
        Task<ClienteEntity> GetById(Guid Id, CancellationToken ct);
        Task Delete(ClienteEntity cliente, CancellationToken ct);
        Task Update(ClienteEntity cliente, CancellationToken ct);
    }
}

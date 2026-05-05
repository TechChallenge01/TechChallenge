using Domain.ValueObjects;

namespace Domain.Aggregates.ClienteAggregates.Repositories
{
    public interface IClienteRepository
    {
        Task<(ICollection<Cliente> Clientes, int Total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task Create(Cliente entity, CancellationToken ct);
        Task<Cliente> GetById(Guid Id, CancellationToken ct);
        Task Delete(Cliente cliente, CancellationToken ct);
        Task<Cliente?> GetByCpf(Cpf cpf, CancellationToken ct = default);
        Task<Cliente?> GetByCnpj(Cnpj cnpj, CancellationToken ct = default);
    }
}

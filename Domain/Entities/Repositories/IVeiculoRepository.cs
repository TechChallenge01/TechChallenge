namespace Domain.Entities.Repositories
{
    public interface IVeiculoRepository
    {
        Task<(List<Veiculo> veiculos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<Veiculo> GetById(Guid id, CancellationToken ct);
        Task Add(Veiculo veiculo, CancellationToken ct);
        Task Delete(Veiculo veiculo, CancellationToken ct);
        Task Update(Veiculo veiculo, CancellationToken ct);
    }
}

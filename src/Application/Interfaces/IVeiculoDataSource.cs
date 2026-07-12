using Shared.DTOs.Veiculos.Input;

namespace Application.Interfaces
{
    public interface IVeiculoDataSource
    {
        Task<(List<VeiculoInputDTO> veiculos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<VeiculoInputDTO>? GetById(Guid id, CancellationToken ct);
        Task Create(VeiculoInputDTO veiculo, CancellationToken ct);
        Task Update(VeiculoInputDTO veiculo, CancellationToken ct);
        Task<VeiculoInputDTO>? GetByPlaca(string placa, CancellationToken ct);
    }
}

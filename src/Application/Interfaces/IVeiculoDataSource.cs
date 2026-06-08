using Shared.DTOs.Veiculos.Input;
using Shared.DTOs.Veiculos.Output;

namespace Application.Interfaces
{
    public interface IVeiculoDataSource
    {
        Task<(List<VeiculoOutputDTO> veiculos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<VeiculoOutputDTO>? GetById(Guid id, CancellationToken ct);
        Task Create(VeiculoInputDTO veiculo, CancellationToken ct);
        Task Update(VeiculoInputDTO veiculo, CancellationToken ct);
    }
}

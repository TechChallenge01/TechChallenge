using Domain.Entities;
using Shared.DTOs.Servicos.Input;

namespace Application.Interfaces
{
    public interface IServicoDataSource
    {
        Task<(List<ServicoInputDTO> servicos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ServicoInputDTO>? GetById(Guid id, CancellationToken ct);
    }
}

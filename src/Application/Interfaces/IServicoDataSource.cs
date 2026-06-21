using Shared.DTOs.Servicos.Input;

namespace Application.Interfaces
{
    public interface IServicoDataSource
    {
        Task<(ICollection<ServicoInputDTO> servicos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ServicoInputDTO>? GetById(Guid id, CancellationToken ct);
        Task<ICollection<ServicoInputDTO>>? GetByIds(ICollection<Guid> id, CancellationToken ct);
        Task Create(ServicoInputDTO servico, CancellationToken ct);
        Task Update(ServicoInputDTO servico, CancellationToken ct);
        Task UpdateServicos(ICollection<ServicoInputDTO> servico, CancellationToken ct);
    }
}

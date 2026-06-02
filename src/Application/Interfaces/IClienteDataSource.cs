using Shared.DTOs.Cliente.Input;

namespace Application.Interfaces
{
    public interface IClienteDataSource
    {
        Task<(List<ClienteInputDTO> clientes, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ClienteInputDTO?> GetById(Guid id, CancellationToken ct);
        Task Create(ClienteInputDTO cliente, CancellationToken ct);
        Task<ClienteInputDTO?> GetByCpf(string cpf, CancellationToken ct);
        Task<ClienteInputDTO?> GetByCnpj(string cnpj, CancellationToken ct);
    }
}

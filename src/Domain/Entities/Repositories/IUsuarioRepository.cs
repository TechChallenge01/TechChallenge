namespace Domain.Entities.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetById(Guid id, CancellationToken ct = default);
    Task Create(Usuario usuario, CancellationToken ct = default);
    Task<Usuario?> GetByEmail(string email, CancellationToken ct = default);
    Task<bool> ExisteEmail(string email, CancellationToken ct = default);
}

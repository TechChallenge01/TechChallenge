using Shared.DTOs.Usuarios.Input;

namespace Application.Interfaces
{
    public interface IUsuarioDataSource
    {
        Task<UsuarioInputDTO?> GetByEmail(string email, CancellationToken ct);
        Task Create(UsuarioInputDTO usuario, CancellationToken ct);
    }
}

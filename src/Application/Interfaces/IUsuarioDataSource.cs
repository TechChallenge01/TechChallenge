using Shared.DTOs.Usuarios.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUsuarioDataSource
    {
        Task<UsuarioInputDTO?> GetByEmail(string email, CancellationToken ct);
        Task Create(UsuarioInputDTO usuario, CancellationToken ct);
    }
}

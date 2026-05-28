using Shared.DTOs.Cliente.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IClienteDataSource
    {
        Task<(List<ClienteInputDTO> clientes, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
    }
}

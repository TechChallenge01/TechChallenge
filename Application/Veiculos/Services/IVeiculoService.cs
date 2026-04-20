using Application.Veiculos.DTOs.Response;
using Shared.Result;
using Shared.Result.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Veiculos.Services
{
    public interface IVeiculoService
    {
        Task<ICommandResult<PagedResultDTO<VeiculoResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
    }
}

using Application.Insumos.DTOs.Requests;
using Application.Insumos.DTOs.Responses;
using Shared.DTOs;
using Shared.DTOs.Insumo.Input;
using Shared.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IInsumoDataSource
{
    Task<(List<InsumoInputDTO> insumos, int total)> GetPaginated(int page, int pageSize, CancellationToken cancellationToken);
    Task<InsumoInputDTO> GetById(Guid id, CancellationToken cancellationToken);
    Task Create(InsumoInputDTO request, CancellationToken cancellationToken);
    Task Update(InsumoInputDTO request, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}

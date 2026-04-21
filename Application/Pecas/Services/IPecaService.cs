using Application.Pecas.DTOs.Requests;
using Application.Pecas.DTOs.Responses;
using Shared.Result;
using Shared.Result.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Pecas.Services
{
    public interface IPecaService
    {
        Task<ICommandResult<PagedResultDTO<PecaResponseDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct);
        Task<ICommandResult<Guid>> Create(PecaRequestDTO request, CancellationToken ct);
        Task<ICommandResult> Delete(Guid id, CancellationToken ct);
        Task<ICommandResult> Update(Guid id, PecaRequestDTO request, CancellationToken ct);
    }
}

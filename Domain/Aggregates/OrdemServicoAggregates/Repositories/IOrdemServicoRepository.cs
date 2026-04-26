using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Aggregates.OrdemServicoAggregates.Repositories;

public interface IOrdemServicoRepository
{
    Task<(ICollection<OrdemServico> OrdemServicos, int Total)> GetPaginated(int page, int pageSize, CancellationToken ct);
    Task<Guid> Create(OrdemServico ordemServico, CancellationToken ct);
    Task<OrdemServico> GetById(int id, CancellationToken ct);
    Task Update(OrdemServico ordemServico, CancellationToken ct);
    Task<ICollection<TimeSpan>> GetByIdsSTimeSpanDataExecucao(ICollection<Guid> ids, CancellationToken ct);
}

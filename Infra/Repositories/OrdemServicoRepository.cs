using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Aggregates.OrdemServicoAggregates.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class OrdemServicoRepository : IOrdemServicoRepository
{
    private readonly AppDbContext _appDbContext;
    public OrdemServicoRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<Guid> Create(OrdemServico ordemServico, CancellationToken ct)
    {
        await _appDbContext.OrdensServico.AddAsync(ordemServico, ct);
        await _appDbContext.SaveChangesAsync(ct);
        return ordemServico.Id;
    }

    public async Task<OrdemServico?> GetById(Guid id, CancellationToken ct)
    {
        return await _appDbContext.OrdensServico
                    .Include(os => os.Servicos)
                    .Include(os => os.Pecas)
                    .FirstOrDefaultAsync(os => os.Id == id && os.Ativo, ct);
    }

    public Task<ICollection<TimeSpan>> GetByIdsSTimeSpanDataExecucao(ICollection<Guid> ids, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<(ICollection<OrdemServico> OrdemServicos, int Total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<OrdemServico> query = _appDbContext.OrdensServico.Where(os => os.Ativo);

        var ordemServicos = await query.Skip((page - 1) * pageSize)
                                       .Take(pageSize)
                                       .AsNoTracking()
                                       .ToListAsync(ct);

        var total = await query.CountAsync(ct);

        return (ordemServicos, total);
    }
}

using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class EstoqueRepository : IEstoqueRepository
{
    private readonly AppDbContext _appDbContext;
    public EstoqueRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Create(Estoque estoque, CancellationToken ct)
    {
        await _appDbContext.Estoques.AddAsync(estoque, ct);
        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<Estoque?> GetById(Guid id, CancellationToken ct)
    {
        return await _appDbContext.Estoques
                                  .Include(e => e.Historicos)
                                  .FirstOrDefaultAsync(e => e.Id == id && e.Ativo, ct);
    }

    public async Task<Estoque?> GetByInsumoId(Guid Insumo, CancellationToken ct)
    {
        return await _appDbContext.Estoques
                                  .Include(e => e.Historicos)
                                  .FirstOrDefaultAsync(e => e.InsumoId == Insumo && e.Ativo, ct);
    }

    public async Task<ICollection<Estoque>> GetByInsumoIds(ICollection<Guid> Insumos, CancellationToken ct)
    {
        return await _appDbContext.Estoques
                                  .Where(e => Insumos.Contains((Guid)e.InsumoId) && e.Ativo)
                                  .Include(e => e.Historicos)
                                  .ToListAsync(ct);
    }

    public async Task<Estoque> GetByPecaId(Guid Peca, CancellationToken ct)
    {
        return await _appDbContext.Estoques
                                  .Include(e => e.Historicos)
                                  .FirstOrDefaultAsync(e => e.PecaId == Peca && e.Ativo, ct);
    }

    public async Task<ICollection<Estoque>> GetByPecaIds(ICollection<Guid> Pecas, CancellationToken ct)
    {
        return await _appDbContext.Estoques
                                  .Where(e => Pecas.Contains((Guid)e.PecaId) && e.Ativo)
                                  .Include(e => e.Historicos)
                                  .ToListAsync(ct);
    }

    public async Task<(ICollection<Estoque> estoques, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Estoque> query = _appDbContext.Estoques.Where(e => e.Ativo);

        var estoque = await query.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsNoTracking()
                                 .ToListAsync(ct);

        var total = await query.CountAsync(ct);

        return (estoque, total);
    }
}

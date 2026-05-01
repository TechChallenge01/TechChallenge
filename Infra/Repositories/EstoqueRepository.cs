using Domain.Aggregates.EstoqueAggregates;
using Domain.Aggregates.EstoqueAggregates.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

    public async Task<Estoque> GetById(Guid id, CancellationToken ct)
    {
        return await _appDbContext.Estoques.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<Estoque> GetByPecaId(Guid Peca, CancellationToken ct)
    {
        return await _appDbContext.Estoques.FirstOrDefaultAsync(e => e.PecaId == Peca, ct);
    }

    public async Task<ICollection<Estoque>> GetByPecaIds(ICollection<Guid> Pecas, CancellationToken ct)
    {
        return await _appDbContext.Estoques.Where(e => Pecas.Contains(e.PecaId)).ToListAsync(ct);
    }

    public async Task<(ICollection<Estoque> estoques, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        var estoque = await _appDbContext.Estoques
                                               .Skip((page - 1) * pageSize)
                                               .Take(pageSize)
                                               .AsNoTracking()
                                               .ToListAsync(ct);

        var total = await _appDbContext.Estoques.CountAsync(ct);

        return (estoque, total);
    }

    public async Task Update(Estoque estoque, CancellationToken ct)
    {
        _appDbContext.Estoques.Update(estoque);
        await _appDbContext.SaveChangesAsync(ct);
    }
}

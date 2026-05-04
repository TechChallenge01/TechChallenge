using Domain.Entities;
using Domain.Entities.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class PecaRepository : IPecaRepository
    {
        private readonly AppDbContext _appDbContext;

        public PecaRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Peca peca, CancellationToken ct)
        {
            await _appDbContext.Pecas.AddAsync(peca, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task Delete(Peca peca, CancellationToken ct)
        {
            _appDbContext.Pecas.Remove(peca);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<Peca?> GetById(Guid id, CancellationToken ct)
        {
            return await _appDbContext.Pecas
                        .FirstOrDefaultAsync(p => p.Id == id && p.Ativo, ct);
        }

        public async Task<List<Peca>> GetByIds(List<Guid> idsPecas, CancellationToken ct)
        {
            return await _appDbContext.Pecas
                        .Where(p => idsPecas.Contains(p.Id) && p.Ativo)
                        .ToListAsync(ct);
        }

        public async Task<(ICollection<Peca> pecas, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<Peca> query = _appDbContext.Pecas.Where(p => p.Ativo);

            var pecas = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync(ct);

            var total = await query.CountAsync(ct);

            return (pecas, total);
        }
    }
}

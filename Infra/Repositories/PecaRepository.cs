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

        public async Task Add(Peca peca, CancellationToken ct)
        {
            await _appDbContext.Pecas.AddAsync(peca, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task Delete(Peca peca, CancellationToken ct)
        {
            _appDbContext.Pecas.Remove(peca);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<Peca> GetById(Guid id, CancellationToken ct)
        {
            return await _appDbContext.Pecas
                        .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<List<Peca>> GetByIds(List<Guid> idsPecas, CancellationToken ct)
        {
            return await _appDbContext.Pecas
                        .Where(p => idsPecas.Contains(p.Id))
                        .ToListAsync(ct);
        }

        public async Task<(ICollection<Peca> pecas, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var pecas = await _appDbContext.Pecas
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToListAsync(ct);

            var total = await _appDbContext.Pecas.CountAsync(ct);

            return (pecas, total);
        }
    }
}

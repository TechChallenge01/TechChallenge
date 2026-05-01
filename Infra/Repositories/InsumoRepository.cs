using Domain.Entities.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class InsumoRepository : IInsumoRepository
    {
        private readonly AppDbContext _appDbContext;

        public InsumoRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Insumo insumo, CancellationToken ct)
        {
            await _appDbContext.Insumos.AddAsync(insumo, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task Delete(Insumo insumo, CancellationToken ct)
        {
            _appDbContext.Insumos.Remove(insumo);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<Insumo> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _appDbContext.Insumos
                        .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<ICollection<Insumo>> GetByIds(ICollection<Guid> ids, CancellationToken cancellationToken)
        {
            return _appDbContext.Insumos.Where(x => ids.Contains(x.Id)).ToList();
        }

        public async Task<(ICollection<Insumo> insumos, int total)> GetPaginatedAsync(int page, int pageSize, CancellationToken ct)
        {
            var insumos = await _appDbContext.Insumos
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .AsNoTracking()
                                .ToListAsync(ct);

            var total = await _appDbContext.Insumos.CountAsync(ct);

            return (insumos, total);
        }
    }
}

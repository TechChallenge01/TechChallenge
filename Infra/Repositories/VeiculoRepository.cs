using Domain.Entities;
using Domain.Entities.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly AppDbContext _appDbContext;

        public VeiculoRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Add(Veiculo veiculo, CancellationToken ct)
        {
            await _appDbContext.Veiculos.AddAsync(veiculo, ct);
        }

        public async Task Delete(Veiculo veiculo, CancellationToken ct)
        {
            _appDbContext.Veiculos.Remove(veiculo);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<Veiculo?> GetById(Guid id, CancellationToken ct)
        {
            return await _appDbContext.Veiculos.FirstOrDefaultAsync(v => v.Id == id && v.Ativo, ct);
        }

        public async Task<(List<Veiculo> veiculos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<Veiculo> query = _appDbContext.Veiculos.Where(v => v.Ativo);

            var veiculos = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .AsNoTracking()
                                      .ToListAsync(ct);

            var total = await query.CountAsync(ct);

            return (veiculos, total);
        }
    }
}

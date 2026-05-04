using Domain.Entities;
using Domain.Entities.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly AppDbContext _appDbContext;

        public ServicoRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Servico servico, CancellationToken ct)
        {
            await _appDbContext.AddAsync(servico, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task Delete(Servico servico, CancellationToken ct)
        {
            _appDbContext.Remove(servico);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<Servico?> GetById(Guid id, CancellationToken ct)
        {
            return await _appDbContext.Servicos
                                      .FirstOrDefaultAsync(s => s.Id == id && s.Ativo, ct);
        }

        public async Task<ICollection<Servico>> GetByIds(ICollection<Guid> idsServicos, CancellationToken ct)
        {
            return await _appDbContext.Servicos
                                      .Where(s => idsServicos.Contains(s.Id) && s.Ativo)
                                      .ToListAsync(ct);
        }

        public async Task<(List<Servico> servicos, int total)> GetPaginatedList(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<Servico> query = _appDbContext.Servicos.Where(s => s.Ativo);

            var servicos = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .AsNoTracking()
                                      .ToListAsync(ct);

            var total = await query.CountAsync(ct);

            return (servicos, total);
        }
    }
}

using Domain.Entities;
using Domain.Entities.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task<Servico> GetById(Guid id, CancellationToken ct)
        {
            return await _appDbContext.Servicos
                                      .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<ICollection<Servico>> GetByIds(ICollection<Guid> idsServicos, CancellationToken ct)
        {
            return await _appDbContext.Servicos
                                      .Where(x => idsServicos.Contains(x.Id))
                                      .ToListAsync(ct);
        }

        public async Task<(List<Servico> servicos, int total)> GetPaginatedList(int page, int pageSize, CancellationToken ct)
        {
            var servicos = await _appDbContext.Servicos
                                              .Skip(page)
                                              .Take(pageSize)
                                              .AsNoTracking()
                                              .ToListAsync(ct);

            var total = await _appDbContext.Servicos.CountAsync(ct);

            return (servicos, total);
        }
    }
}

using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.ClienteAggregates.Repositories;
using Domain.ValueObjects;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _appDbContext;

        public ClienteRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Cliente entity, CancellationToken ct)
        {
            await _appDbContext.Clientes.AddAsync(entity, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task Delete(Cliente cliente, CancellationToken ct)
        {
            _appDbContext.Clientes.Remove(cliente);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<Cliente?> GetByCnpj(Cnpj cnpj, CancellationToken ct = default)
        {
            return await _appDbContext.Clientes
                                        .Include(c => c.Emails)
                                        .Include(c => c.Enderecos)
                                        .Include(c => c.Telefones)
                                        .FirstOrDefaultAsync(c => c.Cnpj != null && c.Cnpj.Valor == cnpj.Valor, ct);
        }

        public async Task<Cliente?> GetByCpf(Cpf cpf, CancellationToken ct = default)
        {
            return await _appDbContext.Clientes
                                        .Include(c => c.Emails)
                                        .Include(c => c.Enderecos)
                                        .Include(c => c.Telefones)
                                        .FirstOrDefaultAsync(c => c.Cpf != null && c.Cpf.Valor == cpf.Valor, ct);
        }

        public async Task<Cliente> GetById(Guid Id, CancellationToken ct)
        {
            return await _appDbContext.Clientes
                                      .Include(c => c.Emails)
                                      .Include(c => c.Enderecos)
                                      .Include(c => c.Telefones)
                                      .Include(c => c.Veiculos)
                                      .FirstOrDefaultAsync(c => c.Id == Id, ct);
        }

        public async Task<(ICollection<Cliente> Clientes, int Total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var clientes = await _appDbContext.Clientes
                                               .Skip((page - 1) * pageSize)
                                               .Take(pageSize)
                                               .AsNoTracking()
                                               .ToListAsync(ct);

            var total = await _appDbContext.Clientes.CountAsync(ct);

            return (clientes, total);
        }
    }
}

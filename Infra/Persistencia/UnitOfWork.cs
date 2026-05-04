using Application.UnitOfWork;
using Infra.Context;

namespace Infra.Persistencia
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _appDbContext.SaveChangesAsync(ct);
        }
    }
}

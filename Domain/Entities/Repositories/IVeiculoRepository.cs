using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Repositories
{
    public interface IVeiculoRepository
    {
        Task<(List<Veiculo> veiculos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct);
    }
}

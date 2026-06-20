using Application.Gateways.Estoques;
using Domain.Aggregates.EstoqueAggregates;

namespace Application.UseCases.Estoques;

public class GetPaginatedUseCase
{
    private readonly EstoqueGateway _estoqueGateway;

    private GetPaginatedUseCase(EstoqueGateway estoqueGateway)
    {
        _estoqueGateway = estoqueGateway;
    }

    public static GetPaginatedUseCase Create(EstoqueGateway estoqueGateway)
    {
        return new GetPaginatedUseCase(estoqueGateway);
    }

    public async Task<(List<Estoque> Estoques, int total)> Run(int page, int pageSize, CancellationToken ct)
    {
        return await _estoqueGateway.GetPaginated(page, pageSize, ct);
    }
}

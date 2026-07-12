using Application.Gateways.Estoques;
using Domain.Aggregates.EstoqueAggregates;

namespace Application.UseCases.Estoques;
public class GetByIdUseCase
{
    private readonly EstoqueGateway _estoqueGateway;

    private GetByIdUseCase(EstoqueGateway estoqueGateway)
    {
        _estoqueGateway = estoqueGateway;
    }

    public static GetByIdUseCase Create(EstoqueGateway estoqueGateway)
    {
        return new GetByIdUseCase(estoqueGateway);
    }

    public async Task<Estoque?> Run(Guid id, CancellationToken ct)
    {
        return await _estoqueGateway.GetById(id, ct);
    }
}

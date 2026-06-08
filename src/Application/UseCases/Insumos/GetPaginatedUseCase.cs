using Application.Gateways.Insumos;

namespace Application.UseCases.Insumos;

public class GetPaginatedUseCase
{
    private readonly InsumoGateway _insumoGateway;

    private GetPaginatedUseCase(InsumoGateway insumoGateway)
    {
        _insumoGateway = insumoGateway;
    }

    public static GetPaginatedUseCase Create(InsumoGateway insumoGateway)
    {
        return new GetPaginatedUseCase(insumoGateway);
    }

    public async Task<(List<Insumo> Insumos, int total)> Run(int page, int pageSize, CancellationToken ct)
    {
        try
        {
            var response = await _insumoGateway.GetPaginated(page, pageSize, ct);
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }
}

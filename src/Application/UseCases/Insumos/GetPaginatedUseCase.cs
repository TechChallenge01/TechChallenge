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
        if (page <= 0)
            throw new ArgumentException("A página deve ser maior que zero.");

        try
        {
            var response = await _insumoGateway.GetPaginated(page, pageSize, ct);
            return response;
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

using Application.Gateways.Insumos;

namespace Application.UseCases.Insumos;

public class GetByIdUseCase
{
    private readonly InsumoGateway _insumoGateway;

    private GetByIdUseCase(InsumoGateway insumoGateway)
    {
        _insumoGateway = insumoGateway;
    }

    public static GetByIdUseCase Create(InsumoGateway insumoGateway)
    {
        return new GetByIdUseCase(insumoGateway);
    }

    public async Task<Insumo?> Run(Guid id, CancellationToken ct)
    {
        try
        {
            var response = await _insumoGateway.GetById(id, ct);
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

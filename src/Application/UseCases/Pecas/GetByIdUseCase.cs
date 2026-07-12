using Application.Gateways.Pecas;
using Domain.Entities;

namespace Application.UseCases.Pecas;

public class GetByIdUseCase
{
    private readonly PecaGateway _pecaGateway;

    private GetByIdUseCase(PecaGateway pecaGateway)
    {
        _pecaGateway = pecaGateway;
    }

    public static GetByIdUseCase Create(PecaGateway pecaGateway)
    {
        return new GetByIdUseCase(pecaGateway);
    }

    public async Task<Peca?> Run(Guid id, CancellationToken ct)
    {
        try
        {
            return await _pecaGateway.GetById(id, ct);
        }
        catch (Exception ex) 
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }
}
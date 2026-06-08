using Application.Gateways.Pecas;
using Domain.Entities;

namespace Application.UseCases.Pecas;

public class GetPaginatedUseCase
{
    private readonly PecaGateway _pecaGateway;

    private GetPaginatedUseCase(PecaGateway pecaGateway)
    {
        _pecaGateway = pecaGateway;
    }

    public static GetPaginatedUseCase Create(PecaGateway pecaGateway)
    {
        return new GetPaginatedUseCase(pecaGateway);
    }

    public async Task<(List<Peca> Pecas, int total)> Run(int page, int pageSize, CancellationToken ct)
    {
        try
        {
            return await _pecaGateway.GetPaginated(page, pageSize, ct);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }
}
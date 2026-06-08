using Application.Gateways.Pecas;
using Application.Pecas.DTOs.Requests;
using Domain.Entities;

namespace Application.UseCases.Pecas;

public class CreateUseCase
{
    private readonly PecaGateway _pecaGateway;

    private CreateUseCase(PecaGateway pecaGateway)
    {
        _pecaGateway = pecaGateway;
    }

    public static CreateUseCase Create(PecaGateway pecaGateway)
    {
        return new CreateUseCase(pecaGateway);
    }

    public async Task<Guid> Run(PecaRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        try 
        {
            var peca = new Peca(request.Nome, request.Descricao, request.MarcaPeca, request.PrecoVenda, idUsuario, DateTime.UtcNow);

            await _pecaGateway.Create(peca, ct);

            return peca.Id;
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
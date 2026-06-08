using Application.Gateways.Pecas;

namespace Application.UseCases.Pecas;

public class DeleteUseCase
{
    private readonly PecaGateway _pecaGateway;

    private DeleteUseCase(PecaGateway pecaGateway)
    {
        _pecaGateway = pecaGateway;
    }

    public static DeleteUseCase Create(PecaGateway pecaGateway)
    {
        return new DeleteUseCase(pecaGateway);
    }

    public async Task Run(Guid idUsuario, Guid id, CancellationToken ct)
    {
        try 
        {
            var peca = await _pecaGateway.GetById(id, ct);

            if (peca is null)
                throw new KeyNotFoundException("Peça não encontrada.");

            peca.Inativar();
            peca.RastrearAlteracao(idUsuario, DateTime.UtcNow);

            await _pecaGateway.Update(peca, ct);
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
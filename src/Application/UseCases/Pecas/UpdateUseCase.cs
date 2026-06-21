using Application.Gateways.Pecas;
using Shared.DTOs.Pecas.Request;

namespace Application.UseCases.Pecas;

public class UpdateUseCase
{
    private readonly PecaGateway _pecaGateway;

    private UpdateUseCase(PecaGateway pecaGateway)
    {
        _pecaGateway = pecaGateway;
    }

    public static UpdateUseCase Create(PecaGateway pecaGateway)
    {
        return new UpdateUseCase(pecaGateway);
    }

    public async Task Run(Guid idUsuario, Guid idPeca, PecaRequestDTO request, CancellationToken ct)
    {
        try 
        {
            var peca = await _pecaGateway.GetById(idPeca, ct);

            if (peca is null)
                throw new KeyNotFoundException("Peça não encontrada.");

            peca.AlterarNome(request.Nome);
            peca.AlterarDescricao(request.Descricao);
            peca.AlterarMarcaPeca(request.MarcaPeca);
            peca.AlterarPrecoVenda(request.PrecoVenda);
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
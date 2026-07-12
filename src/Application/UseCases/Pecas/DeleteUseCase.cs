using Application.Gateways.Estoques;
using Application.Gateways.Pecas;

namespace Application.UseCases.Pecas;

public class DeleteUseCase
{
    private readonly PecaGateway _pecaGateway;
    private readonly EstoqueGateway _estoqueGateway;

    private DeleteUseCase(PecaGateway pecaGateway, EstoqueGateway estoqueGateway)
    {
        _pecaGateway = pecaGateway;
        _estoqueGateway = estoqueGateway;
    }

    public static DeleteUseCase Create(PecaGateway pecaGateway, EstoqueGateway estoqueGateway)
    {
        return new DeleteUseCase(pecaGateway, estoqueGateway);
    }

    public async Task Run(Guid idUsuario, Guid id, CancellationToken ct)
    {
        try
        {
            var peca = await _pecaGateway.GetById(id, ct);

            if (peca is null)
                throw new KeyNotFoundException("Peça não encontrada.");

            var estoque = await _estoqueGateway.GetByPecaId(id, ct);
            if (estoque is not null && estoque.QuantidadeDisponivel > 0)
                throw new ArgumentException("Não é possível excluir uma peça com estoque positivo.");

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

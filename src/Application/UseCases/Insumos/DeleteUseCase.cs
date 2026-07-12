using Application.Gateways.Estoques;
using Application.Gateways.Insumos;

namespace Application.UseCases.Insumos;

public class DeleteUseCase
{
    private readonly InsumoGateway _insumoGateway;
    private readonly EstoqueGateway _estoqueGateway;

    private DeleteUseCase(InsumoGateway insumoGateway, EstoqueGateway estoqueGateway)
    {
        _insumoGateway = insumoGateway;
        _estoqueGateway = estoqueGateway;
    }

    public static DeleteUseCase Create(InsumoGateway insumoGateway, EstoqueGateway estoqueGateway)
    {
        return new DeleteUseCase(insumoGateway, estoqueGateway);
    }

    public async Task Run(Guid idUsuario, Guid id, CancellationToken ct)
    {
        try
        {
            var insumo = await _insumoGateway.GetById(id, ct);

            if (insumo is null)
                throw new KeyNotFoundException("Insumo não encontrado!");

            var estoque = await _estoqueGateway.GetByInsumoId(id, ct);
            if (estoque is not null && estoque.QuantidadeDisponivel > 0)
                throw new ArgumentException("Não é possível excluir um insumo com estoque positivo.");

            insumo.Inativar();
            insumo.RastrearAlteracao(idUsuario, DateTime.UtcNow);

            await _insumoGateway.Update(insumo, ct);
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

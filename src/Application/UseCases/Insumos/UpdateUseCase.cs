using Application.Gateways.Insumos;
using Shared.DTOs.Insumos.Request;

namespace Application.UseCases.Insumos;

public class UpdateUseCase
{
    private readonly InsumoGateway _insumoGateway;

    private UpdateUseCase(InsumoGateway insumoGateway)
    {
        _insumoGateway = insumoGateway;
    }

    public static UpdateUseCase Create(InsumoGateway insumoGateway)
    {
        return new UpdateUseCase(insumoGateway);
    }

    public async Task Run(Guid idUsuario, Guid idInsumo, InsumoRequestDTO insumoRequest, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(insumoRequest.Nome))
                throw new ArgumentException("O nome do insumo é obrigatório.");
            if (insumoRequest.CustoUnitario < 0)
                throw new ArgumentException("O custo unitário não pode ser negativo.");

            var insumo = await _insumoGateway.GetById(idInsumo, ct);

            if (insumo is null)
                throw new KeyNotFoundException("Insumo não encontrado.");

            insumo.AtualizarNome(insumoRequest.Nome);
            insumo.AtualizarDescricao(insumoRequest.Descricao);
            insumo.AtualizarCusto(insumoRequest.CustoUnitario);

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

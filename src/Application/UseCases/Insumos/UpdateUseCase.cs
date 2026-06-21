using Application.Gateways.Insumos;

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
        var insumo = await _insumoGateway.GetById(idInsumo, ct);

        if(insumo is null)
            throw new Exception("Insumo não encontrado.");
        
        insumo.AtualizarNome(insumoRequest.Nome);
        insumo.AtualizarDescricao(insumoRequest.Descricao);
        insumo.AtualizarCusto(insumoRequest.CustoUnitario);

        insumo.RastrearAlteracao(idUsuario, DateTime.UtcNow);

        await _insumoGateway.Update(insumo, ct);
    }
}

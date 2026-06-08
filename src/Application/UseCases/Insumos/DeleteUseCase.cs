using Application.Gateways.Insumos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Insumos;

public class DeleteUseCase
{
    private readonly InsumoGateway _insumoGateway;

    private DeleteUseCase(InsumoGateway insumoGateway)
    {
        _insumoGateway = insumoGateway;
    }

    public static DeleteUseCase Create(InsumoGateway insumoGateway)
    {
        return new DeleteUseCase(insumoGateway);
    }

    public async Task Run(Guid idUsuario, Guid id, CancellationToken ct)
    {
        var insumo = await _insumoGateway.GetById(id, ct);

        if (insumo is null)
            throw new ArgumentNullException("Insumo não encontrado!");

        insumo.Inativar();
        insumo.RastrearAlteracao(idUsuario, DateTime.UtcNow);

        await _insumoGateway.Update(insumo, ct);
    }
}

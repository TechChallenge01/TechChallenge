using Application.Gateways.Insumos;
using Application.Insumos.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Insumos;

public class CreateUseCase
{
    private readonly InsumoGateway _insumoGateway;

    private CreateUseCase(InsumoGateway insumoGateway)
    {
        _insumoGateway = insumoGateway;
    }

    public static CreateUseCase Create(InsumoGateway insumoGateway)
    {
        return new CreateUseCase(insumoGateway);
    }

    public async Task<Guid> Run(InsumoRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        Insumo insumo = new Insumo(request.Nome, request.Descricao, request.CustoUnitario, idUsuario, DateTime.UtcNow);

        await _insumoGateway.Create(insumo, ct);
        return insumo.Id;
    }
}

using API.Extensions;
﻿using Application.Controllers.Pecas;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Shared.Result;

namespace API.EndPoints.Pecas;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/pecas/{id}", async (AppDbContext appDbContext, Guid id, CancellationToken ct) =>
        {
            IPecaDataSource dataSource = new PecaDataSource(appDbContext);
            var controller = new PecaController(dataSource);
            var response = await controller.GetById(id, ct);
            
            return response.ToMinimalResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico", "Almoxarifado"));
    }
}
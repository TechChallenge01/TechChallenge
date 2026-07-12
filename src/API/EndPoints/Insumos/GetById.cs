using API.Extensions;
﻿using Infra.Context;
using Shared.Result;
using Application.Interfaces;
using Infra.DataSources;
using Application.Controllers.Insumos;

namespace API.EndPoints.Insumos;
public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/insumos/{id}", async (AppDbContext appDbContext, Guid id, CancellationToken ct) =>
        {
            IInsumoDataSource dataSource = new InsumoDataSource(appDbContext);
            var controller = new InsumoController(dataSource);
            var response = await controller.GetById(id, ct);

            return response.ToMinimalResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico", "Almoxarifado"));
    }
}

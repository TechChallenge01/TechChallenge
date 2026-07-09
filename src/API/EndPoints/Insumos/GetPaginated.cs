using API.Extensions;
﻿using Infra.Context;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;
using Application.Interfaces;
using Infra.DataSources;
using Application.Controllers.Insumos;

namespace API.EndPoints.Insumos;

public class GetPaginated : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/insumos", async (AppDbContext appDbContext, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
        {
            IInsumoDataSource dataSource = new InsumoDataSource(appDbContext);
            var controller = new InsumoController(dataSource);
            var response = await controller.GetPaginated(page, pageSize, ct);
            
            return response.ToMinimalResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico", "Almoxarifado"));
    }
}

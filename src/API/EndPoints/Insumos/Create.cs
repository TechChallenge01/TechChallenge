using Infra.Context;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Shared.Result;
using Application.Interfaces;
using Infra.DataSources;
using Application.Controllers.Insumos;
using Shared.DTOs.Insumos.Request;

namespace API.EndPoints.Insumos;

public class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/insumos", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] InsumoRequestDTO request, CancellationToken ct) =>
        {
            var idUsuario = httpContext.User.ObterIdUsuario();
            IInsumoDataSource dataSource = new InsumoDataSource(appDbContext);
            var controller = new InsumoController(dataSource);
            var response = await controller.Create(request, idUsuario, ct);

            return response.ToResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Almoxarifado"));
    }
}

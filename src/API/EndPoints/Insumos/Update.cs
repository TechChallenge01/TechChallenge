using Infra.Context;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Shared.Result;
using Infra.DataSources;
using Application.Controllers.Insumos;

namespace API.EndPoints.Insumos;

public class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/insumos/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, [FromBody] InsumoRequestDTO insumoRequest, CancellationToken ct) =>
        {
            var idUsuario = httpContext.User.ObterIdUsuario();
            var dataSource = new InsumoDataSource(appDbContext);
            var controller = new InsumoController(dataSource); 
            var result = await controller.Update(id, insumoRequest, idUsuario, ct);

            return result.ToResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Almoxarifado"));
    }
}

using API.Extensions;
using Application.Controllers.Pecas;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Pecas;

public class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/pecas/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, CancellationToken ct) =>
        {
            var idUsuario = httpContext.User.ObterIdUsuario();
            IPecaDataSource dataSource = new PecaDataSource(appDbContext);
            var controller = new PecaController(dataSource);
            var response = await controller.Delete(id, idUsuario, ct);

            return response.ToResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Almoxarifado"));
    }
}
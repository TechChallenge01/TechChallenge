using Infra.Context;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Shared.Result;
using Application.Interfaces;
using Infra.DataSources;
using Application.Insumos.DTOs.Requests;
using Application.Controllers.Insumo;

namespace API.EndPoints.Insumos;
public class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/insumos/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, CancellationToken ct) =>
        {
            var idUsuario = httpContext.User.ObterIdUsuario();
            IInsumoDataSource dataSource = new InsumoDataSource(appDbContext);
            var controller = new InsumoController(dataSource);
            var response = await controller.Delete(id, idUsuario, ct);
            return response.ToResult();
        });
    }
}

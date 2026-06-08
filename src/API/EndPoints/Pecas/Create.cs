using API.Extensions;
using Application.Controllers.Pecas;
using Application.Interfaces;
using Application.Pecas.DTOs.Requests;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Pecas;

public class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/pecas", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] PecaRequestDTO request, CancellationToken ct) =>
        {
            var idUsuario = httpContext.User.ObterIdUsuario();
            IPecaDataSource dataSource = new PecaDataSource(appDbContext);
            var controller = new PecaController(dataSource);
            var response = await controller.Create(request, idUsuario, ct);

            return response.ToResult();
        });
    }
}
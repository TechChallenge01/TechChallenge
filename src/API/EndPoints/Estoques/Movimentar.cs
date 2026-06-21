using API.Extensions;
using Application.Controllers.Estoques;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Estoques.Request;
using Shared.Result;

namespace API.EndPoints.Estoques;

public class Movimentar : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/estoques/movimentar", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] EstoqueRequestDTO request, CancellationToken ct) =>
        {
            var idUsuario = httpContext.User.ObterIdUsuario();
            IEstoqueDataSource dataSource = new EstoqueDataSource(appDbContext);
            var controller = new EstoqueController(dataSource);
            var response = await controller.Movimentar(request, idUsuario, ct);
            return response.ToResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Almoxarifado"));
    }
}
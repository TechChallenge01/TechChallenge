using API.Extensions;
using Application.Controllers.Pecas;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Pecas;

public class GetPaginated : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/pecas", async (AppDbContext appDbContext, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
        {
            IPecaDataSource dataSource = new PecaDataSource(appDbContext);
            var controller = new PecaController(dataSource);
            var response = await controller.GetPaginated(page, pageSize, ct);
            
            return response.ToMinimalResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico", "Almoxarifado"));
    }
}
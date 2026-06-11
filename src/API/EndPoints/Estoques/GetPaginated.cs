using Application.Controllers.Estoques;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Estoques;

public class GetPaginated : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/estoques", async (AppDbContext appDbContext, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
        {
            IEstoqueDataSource dataSource = new EstoqueDataSource(appDbContext);
            var controller = new EstoqueController(dataSource);
            var response = await controller.GetPaginated(page, pageSize, ct);
            return response.ToResult();
        });
    }
}
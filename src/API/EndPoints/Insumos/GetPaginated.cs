using Infra.Context;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Shared.Result;
using Application.Interfaces;
using Infra.DataSources;
using Application.Insumos.DTOs.Requests;
using Application.Controllers.Insumo;

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
            
            return response.ToResult();
        });
    }
}

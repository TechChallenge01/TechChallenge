using Application.Controllers.Veiculos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Veiculos
{
    public class GetPaginated : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/veiculos", async (AppDbContext appDbContext, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
            {
                IVeiculoDataSource dataSource = new VeiculoDataSource(appDbContext);
                var controller = new VeiculoController(dataSource);
                var response = await controller.GetPaginated(page, pageSize, ct);

                return response.ToResult();
            });
        }
    }
}

using Application.Controllers.OrdensServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class GetById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/ordensservico/{id}", async (AppDbContext appDbContext, [FromRoute] Guid id, CancellationToken ct) =>
            {
                IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                var controller = new OrdemServicoController(dataSource);
                var response = await controller.GetById(id, ct);

                return response.ToResult();
            });
        }
    }
}

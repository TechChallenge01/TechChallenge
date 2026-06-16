using API.EndPoints;
using Application.Gateways.OrdemServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class GetPaginated : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/ordensservico", async (AppDbContext appDbContext, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
            {
                try
                {
                    IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                    var gateway = OrdemServicoGateway.Create(dataSource);
                    var result = await gateway.GetPaginated(page, pageSize, ct);

                    return Results.Ok(new
                    {
                        data = result.ordensServico,
                        page = page,
                        pageSize = result.ordensServico.Count,
                        total = result.total,
                        message = "Ordens de Serviço retornadas com sucesso"
                    });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            });
        }
    }
}

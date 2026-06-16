using API.EndPoints;
using Application.Gateways.OrdemServicos;
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
                try
                {
                    IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                    var gateway = OrdemServicoGateway.Create(dataSource);
                    var result = await gateway.GetById(id, ct);

                    if (result is null)
                        return Results.NotFound(new { message = "Ordem de Serviço não encontrada" });

                    return Results.Ok(new { data = result, message = "Ordem de Serviço retornada com sucesso" });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            });
        }
    }
}

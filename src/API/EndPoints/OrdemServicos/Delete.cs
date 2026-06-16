using API.EndPoints;
using API.Extensions;
using Application.Gateways.OrdemServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class Delete : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/ordensservico/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, CancellationToken ct) =>
            {
                try
                {
                    var idUsuario = httpContext.User.ObterIdUsuario();
                    IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                    var gateway = OrdemServicoGateway.Create(dataSource);

                    await gateway.Delete(id, ct);

                    return Results.Ok(new { message = "Ordem de Serviço deletada com sucesso" });
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.StatusCode(500, new { message = ex.Message });
                }
            });
        }
    }
}

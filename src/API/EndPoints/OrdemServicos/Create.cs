using API.EndPoints;
using API.Extensions;
using Application.Gateways.OrdemServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.OrdemServicos.Input;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class Create : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/ordensservico", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] OrdemServicoRequestDTO request, CancellationToken ct) =>
            {
                try
                {
                    var idUsuario = httpContext.User.ObterIdUsuario();
                    IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                    var gateway = OrdemServicoGateway.Create(dataSource);

                    var ordemServico = new Domain.Aggregates.OrdemServicoAggregates.OrdemServico(request.ClienteId, request.VeiculoId, idUsuario);
                    await gateway.Create(ordemServico, ct);

                    return Results.Created($"/api/ordensservico/{ordemServico.Id}", new { data = ordemServico.Id, message = "Ordem de Serviço criada com sucesso" });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.StatusCode(500, new { message = ex.Message });
                }
            });
        }
    }
}

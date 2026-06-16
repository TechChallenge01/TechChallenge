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
    public class Update : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/ordensservico/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, [FromBody] OrdemServicoUpdateRequestDTO request, CancellationToken ct) =>
            {
                try
                {
                    var idUsuario = httpContext.User.ObterIdUsuario();
                    IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                    var gateway = OrdemServicoGateway.Create(dataSource);

                    var ordemServico = await gateway.GetById(id, ct);
                    if (ordemServico is null)
                        return Results.NotFound(new { message = "Ordem de Serviço não encontrada" });

                    ordemServico.RastrearAlteracao(idUsuario, DateTime.UtcNow);
                    if (!string.IsNullOrWhiteSpace(request.StatusOS))
                        ordemServico.AlterarStatus(request.StatusOS);

                    await gateway.Update(ordemServico, ct);

                    return Results.Ok(new { message = "Ordem de Serviço atualizada com sucesso" });
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

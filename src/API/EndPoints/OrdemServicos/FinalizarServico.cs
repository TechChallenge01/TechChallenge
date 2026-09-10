using API.Extensions;
using Application.Controllers.OrdensServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.OrdemServicos.Request;

namespace API.EndPoints.OrdemServicos
{
    public class FinalizarServico : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/ordemServico/{id}/FinalizarServico", async (AppDbContext appDbContext, IMetricsService metricsService, HttpContext httpContext, Guid id,[FromBody] FinalizarServicoRequestDTO request,CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new OrdemServicoDataSource(appDbContext);
                var servicoDataSource = new ServicoDataSource(appDbContext);

                var controller = new OrdemServicoController(dataSource);

                var response = await controller.FinalizarServico(id, idUsuario, request, servicoDataSource, ct, metricsService);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Mecanico"));
        }
    }
}

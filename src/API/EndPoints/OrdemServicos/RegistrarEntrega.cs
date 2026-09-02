using API.Extensions;
using Application.Controllers.OrdensServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;

namespace API.EndPoints.OrdemServicos
{
    public class RegistrarEntrega : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/ordemServico/{id}/RegistrarEntrega", async (AppDbContext appDbContext, IMetricsService metricsService, HttpContext httpContext, Guid id, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new OrdemServicoDataSource(appDbContext);
                var controller = new OrdemServicoController(dataSource);

                var response = await controller.RealizarEntrega(id, idUsuario, ct, metricsService);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

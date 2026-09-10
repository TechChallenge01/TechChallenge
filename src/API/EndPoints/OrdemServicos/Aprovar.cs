using API.Extensions;
using Application.Controllers.OrdensServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;

namespace API.EndPoints.OrdemServicos
{
    public class Aprovar :IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/ordemServico/{id}/Aprovar", async (AppDbContext appDbContext, IMetricsService metricsService, HttpContext httpContext, Guid id, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var perfil = httpContext.User.ObterPerfil();
                Guid? clienteIdSolicitante = perfil == "Cliente" ? idUsuario : null;

                var dataSource = new OrdemServicoDataSource(appDbContext);
                var pecaDataSource = new PecaDataSource(appDbContext);
                var insumoDataSource = new InsumoDataSource(appDbContext);
                var estoqueDataSource = new EstoqueDataSource(appDbContext);

                var controller = new OrdemServicoController(dataSource);

                var response = await controller.Aprovar(id, idUsuario, pecaDataSource, insumoDataSource, estoqueDataSource, ct, clienteIdSolicitante, metricsService);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Cliente"));
        }
    }
}

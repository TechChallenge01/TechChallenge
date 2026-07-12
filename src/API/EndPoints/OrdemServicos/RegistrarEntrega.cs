using API.Extensions;
using Application.Controllers.OrdensServicos;
using Infra.Context;
using Infra.DataSources;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class RegistrarEntrega : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/ordemServico/{id}/RegistrarEntrega", async (AppDbContext appDbContext, HttpContext httpContext, Guid id, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new OrdemServicoDataSource(appDbContext);
                var controller = new OrdemServicoController(dataSource);

                var response = await controller.RealizarEntrega(id, idUsuario, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

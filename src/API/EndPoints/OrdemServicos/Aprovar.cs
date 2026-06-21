using API.Extensions;
using Application.Controllers.OrdensServicos;
using Infra.Context;
using Infra.DataSources;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class Aprovar :IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/OrdemServico/{id}/Aprovar", async (AppDbContext appDbContext, HttpContext httpContext, Guid id, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new OrdemServicoDataSource(appDbContext);
                var pecaDataSource = new PecaDataSource(appDbContext);
                var insumoDataSource = new InsumoDataSource(appDbContext);
                var estoqueDataSource = new EstoqueDataSource(appDbContext);

                var controller = new OrdemServicoController(dataSource);

                var response = await controller.Aprovar(id, idUsuario, pecaDataSource, insumoDataSource, estoqueDataSource, ct);

                return response.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Cliente"));
        }
    }
}

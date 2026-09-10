using API.Extensions;
using Application.Controllers.OrdensServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;

namespace API.EndPoints.OrdemServicos
{
    public class GetById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/ordemServico/{id}", async (AppDbContext appDbContext, HttpContext httpContext, Guid id, CancellationToken ct) =>
            {
                IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                var controller = new OrdemServicoController(dataSource);

                var perfil = httpContext.User.ObterPerfil();
                Guid? clienteIdSolicitante = perfil == "Cliente" ? httpContext.User.ObterIdUsuario() : null;

                var response = await controller.GetById(id, ct, clienteIdSolicitante);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico", "Cliente"));
        }
    }
}

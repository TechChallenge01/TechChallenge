using API.Extensions;
using Application.Controllers.Veiculos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Shared.Result;

namespace API.EndPoints.Veiculos
{
    public class Delete : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/veiculos/{id}", async (AppDbContext appDbContext, HttpContext httpContext,Guid id, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                IVeiculoDataSource dataSource = new VeiculoDataSource(appDbContext);
                var controller = new VeiculoController(dataSource);
                var response = await controller.Delete(id, idUsuario, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

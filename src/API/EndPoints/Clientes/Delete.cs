using API.Extensions;
using Application.Controllers.Clientes;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Clientes
{
    public class Delete : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/clientes/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                IClienteDataSource dataSource = new ClienteDataSource(appDbContext);
                var controller = new ClienteController(dataSource);
                var response = await controller.Delete(idUsuario, id, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

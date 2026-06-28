using API.Extensions;
using Application.Controllers.Clientes;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Clientes.Request;
using Shared.Result;

namespace API.EndPoints.Clientes
{
    public class Updated : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/clientes/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, [FromBody] ClienteRequestDTO clienteRequest, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new ClienteDataSource(appDbContext);
                var controller = new ClienteController(dataSource);
                var result = await controller.Update(id, clienteRequest, idUsuario, ct);

                return result.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

using Infra.Context;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Shared.Result;
using Application.Interfaces;
using Infra.DataSources;
using Application.Controllers.Clientes;
using Shared.DTOs.Clientes.Request;


namespace API.EndPoints.Clientes
{
    public class Create : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/clientes", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] ClienteRequestDTO request, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                IClienteDataSource dataSource = new ClienteDataSource(appDbContext);
                var controller = new ClienteController(dataSource);
                var response = await controller.Create(request, idUsuario, ct);

                return response.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

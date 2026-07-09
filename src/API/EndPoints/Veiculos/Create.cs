using API.Extensions;
using Application.Controllers.Veiculos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Veiculos.Requests;
using Shared.Result;

namespace API.EndPoints.Veiculos
{
    public class Create : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/veiculos/", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody]VeiculoRequestDTO request, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                IVeiculoDataSource dataSource = new VeiculoDataSource(appDbContext);
                IClienteDataSource clienteDataSource = new ClienteDataSource(appDbContext);

                var controller = new VeiculoController(dataSource);
                var response = await controller.Create(request, idUsuario, clienteDataSource, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

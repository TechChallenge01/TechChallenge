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
    public class Update : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/veiculos/{id}", async (AppDbContext appDbContext, HttpContext httpContext,Guid id, [FromBody] VeiculoRequestDTO request, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                IVeiculoDataSource dataSource = new VeiculoDataSource(appDbContext);
                var controller = new VeiculoController(dataSource);
                var response = await controller.Update(id, idUsuario, request, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

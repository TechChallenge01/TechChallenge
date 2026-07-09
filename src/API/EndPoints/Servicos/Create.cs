using API.Extensions;
using Application.Controllers.Servicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Servicos.Requests;
using Shared.Result;

namespace API.EndPoints.Servicos
{
    public class Create : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/servicos", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] ServicoRequestDTO request, CancellationToken ct) =>
            {
                var IdUsuario = httpContext.User.ObterIdUsuario();
                IServicoDataSource dataSource = new ServicoDataSource(appDbContext);
                var controller = new ServicoController(dataSource);
                var response = await controller.Create(request, IdUsuario, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

using API.Extensions;
using Application.Controllers.Servicos;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Servicos.Requests;
using Shared.Result;

namespace API.EndPoints.Servicos
{
    public class Update : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/servicos/{id}", async (AppDbContext appDbContext, HttpContext httpContext, [FromRoute] Guid id, [FromBody] ServicoRequestDTO request, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new ServicoDataSource(appDbContext);
                var controller = new ServicoController(dataSource);
                var result = await controller.Update(id, request, idUsuario, ct);

                return result.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}


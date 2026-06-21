using API.Extensions;
using Application.Auth.DTOs.Requests;
using Application.Controllers.Usuarios;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Usuarios
{
    public class Create : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/Usuario/", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] CriarUsuarioRequestDTO request, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var usuarioDataSource = new UsuarioDataSource(appDbContext);
                var controller = new UsuarioController(usuarioDataSource);
                var response = await controller.CriarUsuario(request, idUsuario, ct);

                return response.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador"));
        }
    }
}

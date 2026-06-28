using Application.Auth.DTOs.Requests;
using Application.Controllers.Usuarios;
using Infra.Context;
using Infra.DataSources;
using Infra.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Usuarios
{
    public class Login : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/login", async (AppDbContext appDbContext, JwtService jwtService, [FromBody] LoginRequestDTO request, CancellationToken ct) =>
            {
                var dataSource = new UsuarioDataSource(appDbContext);
                var controller = new UsuarioController(dataSource);

                var response = await controller.Login(request, jwtService, ct);

                return response.ToResult();
            });
        }
    }
}

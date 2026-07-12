using API.Extensions;
using Application.Controllers.Clientes;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;

namespace API.EndPoints.Clientes
{
    public class GetById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/clientes/{id}", async (AppDbContext appDbContext, Guid id, CancellationToken ct) =>
            {
                IClienteDataSource dataSource = new ClienteDataSource(appDbContext);
                var controller = new ClienteController(dataSource);
                var response = await controller.GetById(id, ct);
                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

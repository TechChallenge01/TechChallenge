using API.Extensions;
using Application.Controllers.Clientes;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;

namespace API.EndPoints.Clientes
{
    public class GetPaginated : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/clientes", async (AppDbContext appDbContext, [FromQuery] int page  = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
            {
                IClienteDataSource dataSource = new ClienteDataSource(appDbContext);
                var controller = new ClienteController(dataSource);
                var response = await controller.GetPaginated(page, pageSize, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

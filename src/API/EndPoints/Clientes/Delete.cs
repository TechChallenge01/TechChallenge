using Application.Controllers.Clientes;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Clientes
{
    public class Delete : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/clientes/{id}", async (AppDbContext appDbContext,[FromRoute] int id, CancellationToken ct) =>
            {
                IClienteDataSource dataSource = new ClienteDataSource(appDbContext);
                var controller = new ClienteController(dataSource);
                var response = await controller.Delete(id, ct);
                return response.ToResult();
            });
        }
    }
}

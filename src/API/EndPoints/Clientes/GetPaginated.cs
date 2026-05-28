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
            app.MapGet("/api/clientes", async (AppDbContext appDbContext,[FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct) =>
            {
                IClienteDataSource dataSource = new ClienteDataSource(appDbContext);
                return;
            });
        }
    }
}

using Application.Controllers.Servicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.EndPoints.Servicos
{
    public class GetPaginated : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/Servicos/", async (AppDbContext appDbContext, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
            {
                IServicoDataSource dataSource = new ServicoDataSource(appDbContext);
                var controller = new ServicoController(dataSource);
                var response = await controller.GetPaginated(page, pageSize, ct);

                return response.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico"));
        }
    }
}

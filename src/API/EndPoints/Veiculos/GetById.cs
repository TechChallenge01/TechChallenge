using Application.Controllers.Veiculos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Shared.Result;

namespace API.EndPoints.Veiculos
{
    public class GetById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Veiculos/{id}", async (AppDbContext appDbContext, Guid id, CancellationToken ct) =>
            {
                IVeiculoDataSource dataSource = new VeiculoDataSource(appDbContext);
                var controller = new VeiculoController(dataSource);
                var response = await controller.GetById(id, ct);

                return response.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

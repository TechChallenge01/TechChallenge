using Application.Controllers.Servicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Shared.Result;

namespace API.EndPoints.Servicos
{
    public class GetById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/servicos/{id}", async (AppDbContext appDbContext, Guid id, CancellationToken ct) =>
            {
                IServicoDataSource dataSource = new ServicoDataSource(appDbContext);
                var controller = new ServicoController(dataSource);
                var response = await controller.GetById(id, ct);

                return response.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico"));
        }
    }
}

using Application.Controllers.Estoques;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Shared.Result;

namespace API.EndPoints.Estoques;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/estoques/{id}", async (AppDbContext appDbContext, Guid id, CancellationToken ct) =>
        {
            IEstoqueDataSource dataSource = new EstoqueDataSource(appDbContext);
            var controller = new EstoqueController(dataSource);
            var response = await controller.GetById(id, ct);
            return response.ToResult();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario", "Mecanico", "Almoxarifado"));
    }
}
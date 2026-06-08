using Infra.Context;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Shared.Result;
using Application.Interfaces;
using Infra.DataSources;
using Application.Insumos.DTOs.Requests;
using Application.Controllers.Insumo;
using Microsoft.Identity.Client;

namespace API.EndPoints.Insumos;
public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/insumos/{id}", async (AppDbContext appDbContext, Guid id, CancellationToken ct) =>
        {
            IInsumoDataSource dataSource = new InsumoDataSource(appDbContext);
            var controller = new InsumoController(dataSource);
            var response = await controller.GetById(id, ct);

            return response.ToResult();
        });
    }
}

using API.Extensions;
using Application.Controllers.OrdensServicos;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.OrdemServicos.Request;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class FinalizarServico : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/OrdemServico/{id}/Aprovar", async (AppDbContext appDbContext, HttpContext httpContext, Guid id,[FromBody] FinalizarServicoRequestDTO request,CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new OrdemServicoDataSource(appDbContext);
                var servicoDataSource = new ServicoDataSource(appDbContext);

                var controller = new OrdemServicoController(dataSource);

                var response = await controller.FinalizarServico(id, idUsuario, request, servicoDataSource, ct);

                return response.ToResult();
            });
        }
    }
}

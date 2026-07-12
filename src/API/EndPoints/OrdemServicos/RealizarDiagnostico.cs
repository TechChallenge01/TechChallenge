using API.Extensions;
using Application.Controllers.OrdensServicos;
using Infra.Context;
using Infra.DataSources;
using Infra.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.OrdemServicos.Request;

namespace API.EndPoints.OrdemServicos
{
    public class RealizarDiagnostico : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/ordemServico/{id}/RealizarDiagnostico", async (AppDbContext appDbContext, EmailService emailService, HttpContext httpContext, Guid id, [FromBody] DiagnosticoRequestDTO request, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                var dataSource = new OrdemServicoDataSource(appDbContext);
                var pecaDataSource = new PecaDataSource(appDbContext);
                var servicoDataSource = new ServicoDataSource(appDbContext);
                var insumoDataSource = new InsumoDataSource(appDbContext);
                var estoqueDataSource = new EstoqueDataSource(appDbContext);
                var clienteDataSource = new ClienteDataSource(appDbContext);


                var controller = new OrdemServicoController(dataSource);

                var response = await controller.RealizarDiagnostico(id, idUsuario, request, pecaDataSource, servicoDataSource, insumoDataSource, estoqueDataSource, clienteDataSource, emailService, ct);

                return response.ToMinimalResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Mecanico"));
        }
    }
}

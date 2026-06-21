using API.Extensions;
using Application.Controllers.OrdensServicos;
using Application.Interfaces;
using Infra.Context;
using Infra.DataSources;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.OrdemServicos.Request;
using Shared.Result;

namespace API.EndPoints.OrdemServicos
{
    public class Create : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/OrdemServico", async (AppDbContext appDbContext, HttpContext httpContext, [FromBody] OrdemServicoRequestDTO request, CancellationToken ct) =>
            {
                var idUsuario = httpContext.User.ObterIdUsuario();
                IOrdemServicoDataSource dataSource = new OrdemServicoDataSource(appDbContext);
                IClienteDataSource clienteDataSource = new ClienteDataSource(appDbContext);
                IVeiculoDataSource veiculoDataSource = new VeiculoDataSource(appDbContext);
                IPecaDataSource pecaDataSource = new PecaDataSource(appDbContext);
                IServicoDataSource servicoDataSource = new ServicoDataSource(appDbContext);
                IInsumoDataSource insumoDataSource = new InsumoDataSource(appDbContext);
                IEstoqueDataSource estoqueDataSource = new EstoqueDataSource(appDbContext);


                var controller = new OrdemServicoController(dataSource);
                var response = await controller.Create(request, idUsuario,clienteDataSource, veiculoDataSource, pecaDataSource, servicoDataSource, insumoDataSource, estoqueDataSource, ct);

                return response.ToResult();
            }).RequireAuthorization(policy => policy.RequireRole("Administrador", "Funcionario"));
        }
    }
}

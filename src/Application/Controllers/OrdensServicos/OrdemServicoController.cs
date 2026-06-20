using Application.Gateways.Clientes;
using Application.Gateways.Estoques;
using Application.Gateways.Insumos;
using Application.Gateways.OrdemServicos;
using Application.Gateways.Pecas;
using Application.Gateways.Servicos;
using Application.Gateways.Veiculos;
using Application.Interfaces;
using Application.Presenters.OrdensServicos;
using Application.UseCases.OrdensServicos;
using Shared.DTOs;
using Shared.DTOs.OrdemServicos.Output;
using Shared.DTOs.OrdemServicos.Request;
using Shared.Result;

namespace Application.Controllers.OrdensServicos
{
    public class OrdemServicoController
    {
        private readonly IOrdemServicoDataSource _dataSource;

        public OrdemServicoController(IOrdemServicoDataSource ordemServicoDataSource)
        {
            _dataSource = ordemServicoDataSource;
        }

        public async Task<ICommandResult<PagedResultDTO<OrdemServicoOutputDTO>>> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var presenter = new OrdemServicoPresenter("Pesquisa de Ordens de serviços retornada com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var useCase = GetPaginatedUseCase.Create(ordemServicoGateway);
                var response = await useCase.Run(page, pageSize, ct);

                return presenter.TransformPaged(response.ordensServicos, page, response.total);
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest<PagedResultDTO<OrdemServicoOutputDTO>>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<PagedResultDTO<OrdemServicoOutputDTO>>(ex.Message);
            }
        }

        public async Task<ICommandResult<OrdemServicoOutputDTO>> GetById(Guid id, CancellationToken ct)
        {
            var presenter = new OrdemServicoPresenter("Ordem de serviço retornada com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var useCase = GetByIdUseCase.Create(ordemServicoGateway);
                var response = await useCase.Run(id, ct);

                if (response is null)
                    return presenter.NotFound<OrdemServicoOutputDTO>("Ordem de serviço não encontrada");

                return presenter.TransformObject(response);
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest<OrdemServicoOutputDTO>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<OrdemServicoOutputDTO>(ex.Message);
            }
        }

        public async Task<ICommandResult<Guid>> Create(OrdemServicoRequestDTO request, Guid idUsuario, IClienteDataSource clienteDataSource, IVeiculoDataSource veiculoDataSource, IPecaDataSource pecaDataSource, IServicoDataSource servicoDataSource, IInsumoDataSource insumoDataSource, IEstoqueDataSource estoqueDataSource, CancellationToken ct)
        {
            var presenter = new OrdemServicoPresenter("Ordem de serviço criada com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var clienteGateway = ClienteGateway.Create(clienteDataSource);
                var veiculoGateway = VeiculoGateway.Create(veiculoDataSource);
                var pecaGateway = PecaGateway.Create(pecaDataSource);
                var servicoGateway = ServicoGateway.Create(servicoDataSource);
                var insumoGateway = InsumoGateway.Create(insumoDataSource);
                var estoqueGateway = EstoqueGateway.Create(estoqueDataSource);

                var useCase = CreateUseCase.Create(ordemServicoGateway, clienteGateway, veiculoGateway, pecaGateway, servicoGateway, insumoGateway, estoqueGateway);
                var response = await useCase.Run(request, idUsuario, ct);

                return presenter.Created<Guid>(response);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                return presenter.InternalError<Guid>(ex.Message);
            }

        }
        public async Task<ICommandResult> Aprovar(Guid id, Guid idUsuario, IPecaDataSource pecaDataSource, IInsumoDataSource insumoDataSource, IEstoqueDataSource estoqueDataSource, CancellationToken ct)
        {
            var presenter = new OrdemServicoPresenter("Ordem de serviço aprovada com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var pecaGateway = PecaGateway.Create(pecaDataSource);
                var insumoGateway = InsumoGateway.Create(insumoDataSource);
                var estoqueGateway = EstoqueGateway.Create(estoqueDataSource);

                var useCase = AprovarOrdemServicoUseCase.Create(ordemServicoGateway, pecaGateway, insumoGateway, estoqueGateway);
                await useCase.Run(id, idUsuario, ct);

                return presenter.NoContent();
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch(KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return presenter.InternalError(ex.Message);
            }
        }
        public async Task<ICommandResult> Cancelar(Guid id, Guid idUsuario, IPecaDataSource pecaDataSource, IInsumoDataSource insumoDataSource, IEstoqueDataSource estoqueDataSource, CancellationToken ct)
        {
            var presenter = new OrdemServicoPresenter("Ordem de serviço cancelada com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var pecaGateway = PecaGateway.Create(pecaDataSource);
                var insumoGateway = InsumoGateway.Create(insumoDataSource);
                var estoqueGateway = EstoqueGateway.Create(estoqueDataSource);

                var useCase = CancelarOrdemServicoUseCase.Create(ordemServicoGateway, pecaGateway, insumoGateway, estoqueGateway);
                await useCase.Run(id, idUsuario, ct);

                return presenter.NoContent();
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch(KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return presenter.InternalError(ex.Message);
            }
        }
    }
}

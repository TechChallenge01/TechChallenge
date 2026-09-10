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

        public async Task<ICommandResult<OrdemServicoOutputDTO>> GetById(Guid id, CancellationToken ct, Guid? clienteIdSolicitante = null)
        {
            var presenter = new OrdemServicoPresenter("Ordem de serviço retornada com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var useCase = GetByIdUseCase.Create(ordemServicoGateway);
                var response = await useCase.Run(id, ct, clienteIdSolicitante);

                if (response is null)
                    return presenter.NotFound<OrdemServicoOutputDTO>("Ordem de serviço não encontrada");

                return presenter.TransformObject(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return presenter.Forbidden<OrdemServicoOutputDTO>(ex.Message);
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

        public async Task<ICommandResult<Guid>> Create(OrdemServicoRequestDTO request, Guid idUsuario, IClienteDataSource clienteDataSource, IVeiculoDataSource veiculoDataSource, IPecaDataSource pecaDataSource, IServicoDataSource servicoDataSource, IInsumoDataSource insumoDataSource, IEstoqueDataSource estoqueDataSource, CancellationToken ct, IMetricsService? metricsService = null)
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

                metricsService?.IncrementOrdemServicoCriada();

                return presenter.Created<Guid>(response);
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest<Guid>(ex.Message);
            }
            catch (Exception ex)
            {
                metricsService?.IncrementErro(nameof(Create));
                return presenter.InternalError<Guid>(ex.Message);
            }

        }
        public async Task<ICommandResult> Aprovar(Guid id, Guid idUsuario, IPecaDataSource pecaDataSource, IInsumoDataSource insumoDataSource, IEstoqueDataSource estoqueDataSource, CancellationToken ct, Guid? clienteIdSolicitante = null, IMetricsService? metricsService = null)
        {
            var presenter = new OrdemServicoPresenter("Ordem de serviço aprovada com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var pecaGateway = PecaGateway.Create(pecaDataSource);
                var insumoGateway = InsumoGateway.Create(insumoDataSource);
                var estoqueGateway = EstoqueGateway.Create(estoqueDataSource);

                var useCase = AprovarOrdemServicoUseCase.Create(ordemServicoGateway, pecaGateway, insumoGateway, estoqueGateway);
                await useCase.Run(id, idUsuario, ct, clienteIdSolicitante);

                metricsService?.IncrementOrdemServicoStatus("EmExecucao");

                return presenter.NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return presenter.Forbidden(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch(KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                metricsService?.IncrementErro(nameof(Aprovar));
                return presenter.InternalError(ex.Message);
            }
        }
        public async Task<ICommandResult> Cancelar(Guid id, Guid idUsuario, IPecaDataSource pecaDataSource, IInsumoDataSource insumoDataSource, IEstoqueDataSource estoqueDataSource, CancellationToken ct, IMetricsService? metricsService = null)
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

                metricsService?.IncrementOrdemServicoStatus("Cancelada");

                return presenter.NoContent();
            }
            catch(ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch(KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                metricsService?.IncrementErro(nameof(Cancelar));
                return presenter.InternalError(ex.Message);
            }
        }
        public async Task<ICommandResult> FinalizarServico(Guid id, Guid idUsuario, FinalizarServicoRequestDTO request, IServicoDataSource servicoDataSource, CancellationToken ct, IMetricsService? metricsService = null)
        {
            var presenter = new OrdemServicoPresenter("Serviços finalizados com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var servicoGateway = ServicoGateway.Create(servicoDataSource);

                var useCase = FinalizarServicoUseCase.Create(ordemServicoGateway, servicoGateway);
                var ordemServico = await useCase.Run(request, id, idUsuario, ct);

                metricsService?.IncrementOrdemServicoStatus(ordemServico.StatusOS);
                if (ordemServico.StatusOS == "Finalizada")
                    metricsService?.RecordTempoExecucao("Finalizada", ordemServico.TempoExecucao);

                return presenter.NoContent();

            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                metricsService?.IncrementErro(nameof(FinalizarServico));
                return presenter.InternalError(ex.Message);
            }
        }
        public async Task<ICommandResult> RealizarEntrega(Guid id, Guid idUsuario, CancellationToken ct, IMetricsService? metricsService = null)
        {
            var presenter = new OrdemServicoPresenter("Ordem de serviço Entregue!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var useCase = RegistrarEntregaUseCase.Create(ordemServicoGateway);

                await useCase.Run(id, idUsuario, ct);

                metricsService?.IncrementOrdemServicoStatus("Entregue");

                return presenter.NoContent();
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                metricsService?.IncrementErro(nameof(RealizarEntrega));
                return presenter.InternalError(ex.Message);
            }
        }
        public async Task<ICommandResult> IniciarDiagnostico(Guid id, Guid idUsuario, CancellationToken ct, IMetricsService? metricsService = null)
        {
            var presenter = new OrdemServicoPresenter("Diagnóstico iniciado com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var useCase = IniciarDiagnosticoUseCase.Create(ordemServicoGateway);

                await useCase.Run(id, idUsuario, ct);

                metricsService?.IncrementOrdemServicoStatus("EmDiagnostico");

                return presenter.NoContent();

            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                metricsService?.IncrementErro(nameof(IniciarDiagnostico));
                return presenter.InternalError(ex.Message);
            }
        }
        public async Task<ICommandResult> RealizarDiagnostico(Guid id, Guid idUsuario, DiagnosticoRequestDTO request, IPecaDataSource pecaDataSource, IServicoDataSource servicoDataSource, IInsumoDataSource insumoDataSource, IEstoqueDataSource estoqueDataSource, IClienteDataSource clienteDataSource, IEmailService emailService, CancellationToken ct, IMetricsService? metricsService = null)
        {
            var presenter = new OrdemServicoPresenter("Diagnóstico realizado com sucesso!");
            try
            {
                var ordemServicoGateway = OrdemServicoGateway.Create(_dataSource);
                var pecaGateway = PecaGateway.Create(pecaDataSource);
                var servicoGateway = ServicoGateway.Create(servicoDataSource);
                var insumoGateway = InsumoGateway.Create(insumoDataSource);
                var estoqueGateway = EstoqueGateway.Create(estoqueDataSource);
                var clienteGateway = ClienteGateway.Create(clienteDataSource);

                var useCase = RealizarDiagnosticoUseCase.Create(ordemServicoGateway, pecaGateway, servicoGateway, insumoGateway, estoqueGateway, clienteGateway, emailService);
                await useCase.Run(id, idUsuario, request, ct);

                metricsService?.IncrementOrdemServicoStatus("AguardandoAprovacao");

                return presenter.NoContent();
            }
            catch (ArgumentException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return presenter.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return presenter.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                metricsService?.IncrementErro(nameof(RealizarDiagnostico));
                return presenter.InternalError(ex.Message);
            }
        }
    }
}

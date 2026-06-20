using Application.Gateways.Clientes;
using Application.Gateways.Estoques;
using Application.Gateways.Insumos;
using Application.Gateways.OrdemServicos;
using Application.Gateways.Pecas;
using Application.Gateways.Servicos;
using Application.Gateways.Veiculos;
using Domain.Aggregates.OrdemServicoAggregates;
using Shared.DTOs.OrdemServicos.Request;

namespace Application.UseCases.OrdensServicos
{
    public class CreateUseCase
    {
        private readonly OrdemServicoGateway _ordemServicoGateway;
        private readonly ClienteGateway _clienteGateway;
        private readonly VeiculoGateway _veiculoGateway;
        private readonly PecaGateway _pecaGateway;
        private readonly ServicoGateway _servicoGateway;
        private readonly InsumoGateway _insumoGateway;
        private readonly EstoqueGateway _estoqueGateway;

        private CreateUseCase(OrdemServicoGateway ordemServicoGateway, ClienteGateway clienteGateway, VeiculoGateway veiculoGateway, PecaGateway pecaGateway, ServicoGateway servicoGateway, InsumoGateway insumoGateway, EstoqueGateway estoqueGateway)
        {
            _ordemServicoGateway = ordemServicoGateway;
            _clienteGateway = clienteGateway;
            _veiculoGateway = veiculoGateway;
            _pecaGateway = pecaGateway;
            _servicoGateway = servicoGateway;
            _insumoGateway = insumoGateway;
            _estoqueGateway = estoqueGateway;
        }

        public static CreateUseCase Create(OrdemServicoGateway ordemServicoGateway, ClienteGateway clienteGateway, VeiculoGateway veiculoGateway, PecaGateway pecaGateway, ServicoGateway servicoGateway, InsumoGateway insumoGateway, EstoqueGateway estoqueGateway)
        {
            return new CreateUseCase(ordemServicoGateway, clienteGateway, veiculoGateway, pecaGateway, servicoGateway, insumoGateway, estoqueGateway);
        }

        public async Task<Guid> Run(OrdemServicoRequestDTO request, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var validarClienteUseCase = ValidarClienteOrdemServicoUseCase.Create(_clienteGateway);
                var cliente = await validarClienteUseCase.Run(request.Cliente, idUsuario, ct);

                var ValidarVeiculoUseCase = ValidarVeiculoOrdemServicoUseCase.Create(_veiculoGateway);
                var veiculo = await ValidarVeiculoUseCase.Run(cliente, request.Veiculo, idUsuario, ct);

                var ordemServico = new OrdemServico(cliente.Id, veiculo.Id, idUsuario);

                if (request.Pecas.Any())
                {
                    var adicionarPecaUseCase = AdicionarPecasOrdemServicoUseCase.Create(_pecaGateway, _estoqueGateway);
                    await adicionarPecaUseCase.Run(request.Pecas, ordemServico, idUsuario, ct);
                }

                if (request.Servicos.Any())
                {
                    var adcionarServicoUseCase = AdicionarServicosOrdemServicoUseCase.Create(_servicoGateway);
                    await adcionarServicoUseCase.Run(request.Servicos, ordemServico, idUsuario, ct);
                }

                if (request.Insumos.Any())
                {
                    var adicionarInsumosUseCase = AdicionarInsumosOrdemServicoUseCase.Create(_insumoGateway, _estoqueGateway);
                    await adicionarInsumosUseCase.Run(request.Insumos, ordemServico, idUsuario, ct);
                }

                await _ordemServicoGateway.Create(ordemServico, ct);

                return ordemServico.Id;
            }
            catch(KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

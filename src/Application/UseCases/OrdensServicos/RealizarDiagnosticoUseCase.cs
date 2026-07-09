using Application.Gateways.Clientes;
using Application.Gateways.Estoques;
using Application.Gateways.Insumos;
using Application.Gateways.OrdemServicos;
using Application.Gateways.Pecas;
using Application.Gateways.Servicos;
using Application.Interfaces;
using Domain.Aggregates.EstoqueAggregates;
using Shared.DTOs.OrdemServicos.Request;

namespace Application.UseCases.OrdensServicos
{
    public class RealizarDiagnosticoUseCase
    {
        private readonly OrdemServicoGateway _ordemServicoGateway;
        private readonly PecaGateway _pecaGateway;
        private readonly ServicoGateway _servicoGateway;
        private readonly InsumoGateway _insumoGateway;
        private readonly EstoqueGateway _estoqueGateway;
        private readonly ClienteGateway _clienteGateway;
        private readonly IEmailService _emailService;

        private RealizarDiagnosticoUseCase(OrdemServicoGateway ordemServicoGateway, PecaGateway pecaGateway, ServicoGateway servicoGateway, InsumoGateway insumoGateway, EstoqueGateway estoqueGateway, ClienteGateway clienteGateway, IEmailService emailService)
        {
            _ordemServicoGateway = ordemServicoGateway;
            _pecaGateway = pecaGateway;
            _servicoGateway = servicoGateway;
            _insumoGateway = insumoGateway;
            _estoqueGateway = estoqueGateway;
            _clienteGateway = clienteGateway;
            _emailService = emailService;
        }

        public static RealizarDiagnosticoUseCase Create(OrdemServicoGateway ordemServicoGateway, PecaGateway pecaGateway, ServicoGateway servicoGateway, InsumoGateway insumoGateway, EstoqueGateway estoqueGateway, ClienteGateway clienteGateway, IEmailService emailService)
        {
            return new RealizarDiagnosticoUseCase(ordemServicoGateway, pecaGateway, servicoGateway, insumoGateway, estoqueGateway, clienteGateway, emailService);
        }

        public async Task Run(Guid id, Guid idUsuario, DiagnosticoRequestDTO request, CancellationToken ct)
        { 
            try
            {
                var ordemServico = await _ordemServicoGateway.GetById(id, ct);

                if (ordemServico is null) throw new KeyNotFoundException("ordem de serviço informada não existe!");

                var possuiServico = request.servicos is not null && request.servicos.Any();
                var possuiPeca = request.pecas is not null && request.pecas.Any();
                var possuiInsumos = request.insumos is not null && request.insumos.Any();

                if (!possuiServico && !possuiPeca && !possuiInsumos)
                    throw new ArgumentException("O diagnóstico deve conter ao menos um serviço, uma peça ou um insumo!");

                bool ordemPecas = false;
                var estoques = new List<Estoque>();

                if (possuiPeca)
                {
                    var estoquesReservados = (await _estoqueGateway.GetByPecasIds(ordemServico.Pecas.Select(x => x.PecaId).ToList(), ct)).ToList();

                    estoquesReservados.ForEach(x => x.LiberarReserva(ordemServico.Pecas.FirstOrDefault(y => y.PecaId == x.PecaId).Quantidade, idUsuario));

                    var pecasAgrupadas = request.pecas
                        .GroupBy(p => p.PecaId)
                        .Select(g => new { PecaId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                        .ToList();

                    var idsPecas = pecasAgrupadas.Select(p => p.PecaId).ToList();
                    var pecasEntities = await _pecaGateway.GetByIds(idsPecas, ct);

                    if (pecasEntities.Count() != idsPecas.Count)
                        throw new KeyNotFoundException("Uma ou mais peças não foram encontradas.!"); 

                    estoques.AddRange(await _estoqueGateway.GetByPecasIds(idsPecas, ct));

                    ordemPecas = true;
                }

                bool ordemInsumos = false;

                if (possuiInsumos)
                {
                    var estoquesReservados = (await _estoqueGateway.GetByInsumosIds(ordemServico.Insumos.Select(x => x.InsumoId).ToList(), ct)).ToList();

                    estoquesReservados.ForEach(x => x.LiberarReserva(ordemServico.Insumos.FirstOrDefault(y => y.InsumoId == x.InsumoId).Quantidade, idUsuario));

                    var InsumosAgrupados = request.insumos
                        .GroupBy(s => s.InsumoId)
                        .Select(g => new { InsumoId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                        .ToList();

                    var idsInsumos = InsumosAgrupados.Select(s => s.InsumoId).ToList();
                    var insumosEntities = await _insumoGateway.GetByIds(idsInsumos, ct);

                    if (insumosEntities.Count() != idsInsumos.Count)
                        throw new KeyNotFoundException("Um ou mais insumos não foram encontrados.!");

                    estoques.AddRange(await _estoqueGateway.GetByInsumosIds(idsInsumos, ct));

                    ordemInsumos = true;
                }

                if(estoques.Any())
                    await _estoqueGateway.UpdateEstoques(estoques, ct);

                bool ordemServicos = false;

                if (possuiServico)
                {
                    var servicosAgrupados = request.servicos
                        .GroupBy(s => s.ServicoId)
                        .Select(g => new { ServicoId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                        .ToList();

                    var idsServicos = servicosAgrupados.Select(s => s.ServicoId).ToList();
                    var servicosEntities = await _servicoGateway.GetByIds(idsServicos, ct);

                    if (servicosEntities.Count() != idsServicos.Count)
                        throw new KeyNotFoundException("Um ou mais serviços não foram encontrados!"); 

                    ordemServicos = true;
                }

                if (ordemServicos)
                {
                    var servicoUseCase = AdicionarServicosOrdemServicoUseCase.Create(_servicoGateway);
                    await servicoUseCase.Run(request.servicos, ordemServico, idUsuario, ct);
                }

                if (ordemPecas)
                {
                    var pecaUseCase = AdicionarPecasOrdemServicoUseCase.Create(_pecaGateway, _estoqueGateway);
                    await pecaUseCase.Run(request.pecas, ordemServico, idUsuario, ct);
                }

                if (ordemInsumos)
                {
                    var insumoUseCase = AdicionarInsumosOrdemServicoUseCase.Create(_insumoGateway, _estoqueGateway);
                    await insumoUseCase.Run(request.insumos, ordemServico, idUsuario, ct);
                }

                await _ordemServicoGateway.Update(ordemServico, ct);

                var envioEmailUseCase = EnviarOrcamentoUseCase.Create(_clienteGateway, _emailService);
                await envioEmailUseCase.Run(ordemServico, ct);

            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

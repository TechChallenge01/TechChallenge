using Application.Gateways.OrdemServicos;
using Application.Gateways.Servicos;
using Domain.Aggregates.OrdemServicoAggregates;
using Shared.DTOs.OrdemServicos.Request;

namespace Application.UseCases.OrdensServicos
{
    public class FinalizarServicoUseCase
    {
        private readonly OrdemServicoGateway _ordemServicoGateway;
        private readonly ServicoGateway _servicoGateway;

        private FinalizarServicoUseCase(OrdemServicoGateway ordemServicoGateway, ServicoGateway servicoGateway)
        {
            _ordemServicoGateway = ordemServicoGateway;
            _servicoGateway = servicoGateway;
        }

        public static FinalizarServicoUseCase Create(OrdemServicoGateway ordemServicoGateway, ServicoGateway servicoGateway)
        {
            return new FinalizarServicoUseCase(ordemServicoGateway, servicoGateway);
        }

        public async Task<OrdemServico> Run(FinalizarServicoRequestDTO request, Guid ordemServicoId, Guid usuarioId,CancellationToken ct)
        {
            try
            {
                var ordemServico = await _ordemServicoGateway.GetById(ordemServicoId, ct);

                if (ordemServico is null) throw new KeyNotFoundException("ordem de serviço informada não existe!");

                var servicos = await _servicoGateway.GetByIds(request.servicosId, ct);

                if(servicos.Count() != request.servicosId.Count()) throw new KeyNotFoundException("Um ou mais Serviços não localizados!");

                ordemServico.FinalizarOrdemServico(request.servicosId);

                var temposBanco = await _ordemServicoGateway.GetByIdsSTimeSpanDataExecucao(request.servicosId, ct);

                var temposNovos = ordemServico.Servicos
                                              .Where(s => request.servicosId.Contains(s.ServicoId))
                                              .Select(s => s.DataTerminoExecucao - s.DataInicioExecucao);

                var tempos = temposBanco.Where(t => t.HasValue)
                                        .Select(t => t.Value)
                                        .Concat(
                                            temposNovos
                                                .Where(t => t.HasValue)
                                                .Select(t => t.Value)).ToList();
                

                foreach (var servico in servicos)
                {
                    servico.AtualizarTempoMedio(tempos);
                    servico.RastrearAlteracao(usuarioId, DateTime.UtcNow);
                }

                ordemServico.RastrearAlteracao(usuarioId, DateTime.UtcNow);

                await _servicoGateway.UpdateServicos(servicos, ct);
                await _ordemServicoGateway.Update(ordemServico, ct);

                return ordemServico;
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

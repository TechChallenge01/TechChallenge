using Application.Gateways.Servicos;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.ValueObjects;
using Shared.DTOs.OrdemServicos.Shared;

namespace Application.UseCases.OrdensServicos
{
    public class AdicionarServicosOrdemServicoUseCase
    {
        private readonly ServicoGateway _servicoGateway;

        private AdicionarServicosOrdemServicoUseCase(ServicoGateway servicoGateway)
        {
            _servicoGateway = servicoGateway;
        }

        public static AdicionarServicosOrdemServicoUseCase Create(ServicoGateway servicoGateway)
        {
            return new AdicionarServicosOrdemServicoUseCase(servicoGateway);
        }

        public async Task Run(ICollection<OrdemServicoServicoRequestDTO> servicosRequest, OrdemServico ordemServico, Guid idUsuario, CancellationToken ct)
        {
            if (!servicosRequest.Any())
                return;

            var servicosAgrupados = servicosRequest
                .GroupBy(s => s.ServicoId)
                .Select(g => new { ServicoId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                .ToList();

            var idsServicos = servicosAgrupados.Select(s => s.ServicoId).ToList();
            var servicosEntities = await _servicoGateway.GetByIds(idsServicos, ct);

            if (servicosEntities.Count() != idsServicos.Count)
                throw new KeyNotFoundException("Um ou mais serviços não foram encontrados.");

            var ordemServicos = servicosAgrupados.Select(s =>
            {
                var valorUnitario = servicosEntities.First(e => e.Id == s.ServicoId).ValorUnitario;
                return new OrdemServicoServico(ordemServico.Id, s.ServicoId, s.QuantidadeTotal, valorUnitario);
            }).ToList();

            ordemServico.AlterarServico(ordemServicos);
        }
    }
}

using Application.Gateways.Estoques;
using Application.Gateways.Pecas;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.ValueObjects;
using Shared.DTOs.OrdemServicos.Shared;

namespace Application.UseCases.OrdensServicos
{
    public class AdicionarPecasOrdemServicoUseCase
    {
        private readonly PecaGateway _pecaGateway;
        private readonly EstoqueGateway _estoqueGateway;

        private AdicionarPecasOrdemServicoUseCase(PecaGateway pecaGateway, EstoqueGateway estoqueGateway)
        {
            _pecaGateway = pecaGateway;
            _estoqueGateway = estoqueGateway;
        }

        public static AdicionarPecasOrdemServicoUseCase Create(PecaGateway pecaGateway, EstoqueGateway estoqueGateway)
        {
            return new AdicionarPecasOrdemServicoUseCase(pecaGateway, estoqueGateway);
        }

        public async Task Run(ICollection<OrdemServicoPecaRequestDTO> pecasRequest, OrdemServico ordemServico, Guid idUsuario, CancellationToken ct)
        {
            if (!pecasRequest.Any())
                return;

            var pecasAgrupadas = pecasRequest.GroupBy(p => p.PecaId)
                    .Select(g => new { PecaId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                    .ToList();

            var idsPecas = pecasAgrupadas.Select(p => p.PecaId).ToList();
            var pecasEntities = await _pecaGateway.GetByIds(idsPecas, ct);

            if (pecasEntities.Count() != idsPecas.Count)
                throw new KeyNotFoundException("Uma ou mais peças não foram encontradas.");

            var ordemPecas = pecasAgrupadas.Select(p =>
            {
                var valorUnitario = pecasEntities.First(e => e.Id == p.PecaId).ValorUnitario;
                return new OrdemServicoPeca(ordemServico.Id, p.PecaId, p.QuantidadeTotal, valorUnitario);
            }).ToList();

            ordemServico.AlterarPeca(ordemPecas);

            var estoques = await _estoqueGateway.GetByPecasIds(idsPecas, ct);

            foreach (var peca in ordemPecas)
            {
                var estoque = estoques.FirstOrDefault(e => e.PecaId == peca.PecaId);

                if (estoque is not null)
                {
                    estoque.ReservarEstoque(peca.Quantidade, Guid.NewGuid());
                    await _estoqueGateway.Update(estoque, ct);
                }
            }
        }
    }
}

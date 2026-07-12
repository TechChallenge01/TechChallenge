using Application.Gateways.Estoques;
using Application.Gateways.Insumos;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.ValueObjects;
using Shared.DTOs.OrdemServicos.Shared;

namespace Application.UseCases.OrdensServicos
{
    public class AdicionarInsumosOrdemServicoUseCase
    {
        private readonly InsumoGateway _insumoGateway;
        private readonly EstoqueGateway _estoqueGateway;

        private AdicionarInsumosOrdemServicoUseCase(InsumoGateway pecaGateway, EstoqueGateway estoqueGateway)
        {
            _insumoGateway = pecaGateway;
            _estoqueGateway = estoqueGateway;
        }

        public static AdicionarInsumosOrdemServicoUseCase Create(InsumoGateway insumoGateway, EstoqueGateway estoqueGateway)
        {
            return new AdicionarInsumosOrdemServicoUseCase(insumoGateway, estoqueGateway);
        }

        public async Task Run(ICollection<OrdemServicoInsumoRequestDTO> insumosRequest, OrdemServico ordemServico, Guid idUsuario, CancellationToken ct)
        {
            if (!insumosRequest.Any())
                return;

            var insumosAgrupados = insumosRequest.GroupBy(s => s.InsumoId)
                .Select(g => new { InsumoId = g.Key, QuantidadeTotal = g.Sum(x => x.Quantidade) })
                .ToList();

            var idsInsumos = insumosAgrupados.Select(s => s.InsumoId).ToList();
            var InsumosEntities = await _insumoGateway.GetByIds(idsInsumos, ct);

            if (InsumosEntities.Count() != idsInsumos.Count)
                throw new KeyNotFoundException("Um ou mais insumos não foram encontrados.");

            var ordemInsumos = insumosAgrupados.Select(i =>
            {
                var valorUnitario = InsumosEntities.First(e => e.Id == i.InsumoId).CustoUnitario;
                return new OrdemServicoInsumo(ordemServico.Id, i.InsumoId, i.QuantidadeTotal, valorUnitario);
            }).ToList();

            ordemServico.AlterarInsumo(ordemInsumos);

            var estoques = await _estoqueGateway.GetByInsumosIds(idsInsumos, ct);

            foreach (var insumo in ordemInsumos)
            {
                var estoque = estoques.FirstOrDefault(e => e.InsumoId == insumo.InsumoId);

                if (estoque is not null)
                {
                    estoque.ReservarEstoque(insumo.Quantidade, Guid.NewGuid());
                    await _estoqueGateway.Update(estoque, ct);
                }
            }
        }
    }
}

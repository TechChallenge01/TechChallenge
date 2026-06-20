using Application.Gateways.Estoques;
using Application.Gateways.Insumos;
using Application.Gateways.OrdemServicos;
using Application.Gateways.Pecas;

namespace Application.UseCases.OrdensServicos
{
    public class AprovarOrdemServicoUseCase
    {
        private readonly OrdemServicoGateway _ordemServicoGateway;
        private readonly PecaGateway _pecaGateway;
        private readonly InsumoGateway _insumoGateway;
        private readonly EstoqueGateway _estoqueGateway;

        private AprovarOrdemServicoUseCase(OrdemServicoGateway ordemServicoGateway, PecaGateway pecaGateway, InsumoGateway insumoGateway, EstoqueGateway estoqueGateway)
        {
            _ordemServicoGateway = ordemServicoGateway;
            _pecaGateway = pecaGateway;
            _insumoGateway = insumoGateway;
            _estoqueGateway = estoqueGateway;
        }
        public static AprovarOrdemServicoUseCase Create(OrdemServicoGateway ordemServicoGateway, PecaGateway pecaGateway, InsumoGateway insumoGateway, EstoqueGateway estoqueGateway)
        {
            return new AprovarOrdemServicoUseCase(ordemServicoGateway, pecaGateway, insumoGateway, estoqueGateway);
        }

        public async Task Run(Guid id, Guid idUsuario, CancellationToken ct)
        {
            try
            {
                var ordemServico = await _ordemServicoGateway.GetById(id, ct);

                if (ordemServico is null) throw new KeyNotFoundException("ordem de serviço informada não existe!");

                if (ordemServico.Pecas.Any())
                {
                    var ids = ordemServico.Pecas.Select(p => p.PecaId).ToList();
                    var estoques = await _estoqueGateway.GetByPecasIds(ids, ct);
                    foreach (var estoque in estoques)
                    {
                        estoque.LiberarReserva(ordemServico.Pecas.Where(p => p.PecaId == estoque.PecaId).Sum(p => p.Quantidade), idUsuario);
                        estoque.RetirarEstoque(ordemServico.Pecas.Where(p => p.PecaId == estoque.PecaId).Sum(p => p.Quantidade), idUsuario);
                    }
                    await _estoqueGateway.UpdateEstoques(estoques, ct);
                }

                if (ordemServico.Insumos.Any())
                {
                    var ids = ordemServico.Insumos.Select(p => p.InsumoId).ToList();
                    var estoques = await _estoqueGateway.GetByInsumosIds(ids, ct);

                    foreach (var estoque in estoques)
                    {
                        estoque.LiberarReserva(ordemServico.Insumos.Where(p => p.InsumoId == estoque.InsumoId).Sum(p => p.Quantidade), idUsuario);
                        estoque.RetirarEstoque(ordemServico.Insumos.Where(p => p.InsumoId == estoque.InsumoId).Sum(p => p.Quantidade), idUsuario);
                    }
                    await _estoqueGateway.UpdateEstoques(estoques, ct);
                }

                ordemServico.AprovarOrdemServico();
                ordemServico.RastrearAlteracao(idUsuario, DateTime.UtcNow);

                await _ordemServicoGateway.Update(ordemServico, ct);

            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

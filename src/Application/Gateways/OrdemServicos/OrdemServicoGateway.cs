using Application.Interfaces;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.ValueObjects;
using Shared.DTOs.OrdemServicos.Input;
using Shared.DTOs.OrdemServicos.Shared;

namespace Application.Gateways.OrdemServicos
{
    public class OrdemServicoGateway
    {
        private readonly IOrdemServicoDataSource _dataSource;

        private OrdemServicoGateway(IOrdemServicoDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public static OrdemServicoGateway Create(IOrdemServicoDataSource dataSource)
        {
            return new OrdemServicoGateway(dataSource);
        }

        public async Task<(List<OrdemServico> ordensServico, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var result = await _dataSource.GetPaginated(page, pageSize, ct);

            var ordensServico = result.ordensServico.Select(dto => new OrdemServico(dto.Id, dto.ClienteId, dto.VeiculoId, dto.StatusOS, dto.Observacao, dto.ValorTotal, dto.ValorDesconto, dto.InicioExecucao, dto.TerminoExecucao, 
                dto.Servicos.Select(oss => new OrdemServicoServico(dto.Id, oss.ServicoId, oss.Quantidade, oss.ValorUnitario)).ToList(),
                dto.Pecas.Select(osp => new OrdemServicoPeca(dto.Id, osp.PecaId, osp.Quantidade, osp.ValorUnitario)).ToList(),
                dto.Insumos.Select(osi => new OrdemServicoInsumo(dto.Id, osi.InsumoId, osi.Quantidade, osi.CustoUnitario)).ToList())).ToList();

            return (ordensServico, result.total);
        }

        public async Task<OrdemServico?> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _dataSource.GetById(id, ct);

            if (dto is null)
                return null;

            var ordemServico = new OrdemServico(dto.Id, dto.ClienteId, dto.VeiculoId, dto.StatusOS, dto.Observacao, dto.ValorTotal, dto.ValorDesconto, dto.InicioExecucao, dto.TerminoExecucao,
                                                dto.Servicos.Select(oss => new OrdemServicoServico(dto.Id, oss.ServicoId, oss.Quantidade, oss.ValorUnitario)).ToList(),
                                                dto.Pecas.Select(osp => new OrdemServicoPeca(dto.Id, osp.PecaId, osp.Quantidade, osp.ValorUnitario)).ToList(),
                                                dto.Insumos.Select(osi => new OrdemServicoInsumo(dto.Id, osi.InsumoId, osi.Quantidade, osi.CustoUnitario)).ToList());

            return ordemServico;
        }

        public async Task Create(OrdemServico ordemServico, CancellationToken ct)
        {
            var dto = new OrdemServicoInputDTO
            {
                Id = ordemServico.Id,
                ClienteId = ordemServico.ClienteId,
                VeiculoId = ordemServico.VeiculoId,
                StatusOS = ordemServico.StatusOS,
                Observacao = ordemServico.Observacao,
                ValorTotal = ordemServico.ValorTotal,
                ValorDesconto = ordemServico.ValorDesconto,
                InicioExecucao = ordemServico.InicioExecucao,
                TerminoExecucao = ordemServico.TerminoExecucao,
                IdUsuarioCriacao = ordemServico.IdUsuarioCriacao,
                DataCriacao = ordemServico.DataCriacao,
                IdUsuarioAtualizacao = ordemServico.IdUsuarioAtualizacao,
                DataAtualizacao = ordemServico.DataAtualizacao,
                Insumos = ordemServico.Insumos.Select(i => new OrdemServicoInsumoDTO
                {
                    CustoUnitario = i.CustoUnitario,
                    InsumoId = i.InsumoId,
                    Quantidade = i.Quantidade
                }).ToList(),
                Pecas = ordemServico.Pecas.Select(p => new OrdemServicoPecaDTO
                {
                    PecaId = p.PecaId,
                    Quantidade = p.Quantidade,
                    ValorUnitario = p.ValorUnitario
                }).ToList(),
                Servicos = ordemServico.Servicos.Select(s => new OrdemServicoServicoDTO
                {
                    DataInicioExecucao = s.DataInicioExecucao,
                    DataTerminoExecucao = s.DataTerminoExecucao,
                    Quantidade = s.Quantidade,
                    ServicoId = s.ServicoId,
                    Status = s.Status,
                    ValorUnitario = s.ValorUnitario
                }).ToList()
            };

            await _dataSource.Create(dto, ct);
        }

        public async Task Update(OrdemServico ordemServico, CancellationToken ct)
        {
            var dto = new OrdemServicoInputDTO
            {
                Id = ordemServico.Id,
                ClienteId = ordemServico.ClienteId,
                VeiculoId = ordemServico.VeiculoId,
                StatusOS = ordemServico.StatusOS,
                Observacao = ordemServico.Observacao,
                ValorTotal = ordemServico.ValorTotal,
                ValorDesconto = ordemServico.ValorDesconto,
                InicioExecucao = ordemServico.InicioExecucao,
                TerminoExecucao = ordemServico.TerminoExecucao,
                IdUsuarioCriacao = ordemServico.IdUsuarioCriacao,
                DataCriacao = ordemServico.DataCriacao,
                IdUsuarioAtualizacao = ordemServico.IdUsuarioAtualizacao,
                DataAtualizacao = ordemServico.DataAtualizacao,
                Ativo = ordemServico.Ativo
            };

            await _dataSource.Update(dto, ct);
        }

        public async Task<List<OrdemServico>> GetByClienteId(Guid clienteId, CancellationToken ct)
        {
            var dtos = await _dataSource.GetByClienteId(clienteId, ct);

            return dtos.Select(dto => new OrdemServico(dto.ClienteId, dto.VeiculoId, dto.IdUsuarioCriacao)).ToList();
        }

        public async Task<List<OrdemServico>> GetByStatus(string status, CancellationToken ct)
        {
            var dtos = await _dataSource.GetByStatus(status, ct);

            return dtos.Select(dto => new OrdemServico(dto.ClienteId, dto.VeiculoId, dto.IdUsuarioCriacao)).ToList();
        }

        public async Task<ICollection<TimeSpan?>> GetByIdsSTimeSpanDataExecucao(ICollection<Guid> ids, CancellationToken ct)
        {
            var response = await _dataSource.GetByIdsSTimeSpanDataExecucao(ids, ct);

            return response;
        }
    }
}

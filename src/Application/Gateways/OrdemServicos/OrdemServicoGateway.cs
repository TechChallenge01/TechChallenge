using Application.Interfaces;
using Domain.Aggregates.OrdemServicoAggregates;
using Shared.DTOs.OrdemServicos.Input;

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

            var ordensServico = result.ordensServico.Select(dto => new OrdemServico(dto.ClienteId, dto.VeiculoId, dto.IdUsuarioCriacao)).ToList();

            return (ordensServico, result.total);
        }

        public async Task<OrdemServico?> GetById(Guid id, CancellationToken ct)
        {
            var dto = await _dataSource.GetById(id, ct);

            if (dto is null)
                return null;

            var ordemServico = new OrdemServico(dto.ClienteId, dto.VeiculoId, dto.IdUsuarioCriacao);
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
                Ativo = ordemServico.Ativo
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

        public async Task Delete(Guid id, CancellationToken ct)
        {
            await _dataSource.Delete(id, ct);
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
    }
}

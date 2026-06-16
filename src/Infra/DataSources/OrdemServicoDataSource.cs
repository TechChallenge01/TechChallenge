using Application.Interfaces;
using Infra.Context;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.OrdemServicos.Input;

namespace Infra.DataSources
{
    public class OrdemServicoDataSource : IOrdemServicoDataSource
    {
        private readonly AppDbContext _appDbContext;

        public OrdemServicoDataSource(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(OrdemServicoInputDTO ordemServico, CancellationToken ct)
        {
            var ordemServicoDbModel = new OrdemServicoDbModel(
                ordemServico.Id,
                ordemServico.ClienteId,
                ordemServico.VeiculoId,
                ordemServico.StatusOS,
                ordemServico.Observacao,
                ordemServico.ValorTotal,
                ordemServico.ValorDesconto,
                ordemServico.InicioExecucao,
                ordemServico.TerminoExecucao,
                ordemServico.IdUsuarioCriacao,
                ordemServico.DataCriacao,
                ordemServico.IdUsuarioAtualizacao,
                ordemServico.DataAtualizacao);

            await _appDbContext.OrdensServico.AddAsync(ordemServicoDbModel, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            var ordemServico = await _appDbContext.OrdensServico
                .FirstOrDefaultAsync(os => os.Id == id && os.Ativo, ct);

            if (ordemServico is null)
                throw new KeyNotFoundException("Ordem de Serviço não encontrada");

            ordemServico.Inativar();

            _appDbContext.OrdensServico.Update(ordemServico);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<OrdemServicoInputDTO?> GetById(Guid id, CancellationToken ct)
        {
            var ordemServico = await _appDbContext.OrdensServico
                .FirstOrDefaultAsync(os => os.Id == id && os.Ativo, ct);

            if (ordemServico is null)
                return null;

            return MapToDTO(ordemServico);
        }

        public async Task<(List<OrdemServicoInputDTO> ordensServico, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<OrdemServicoDbModel> query = _appDbContext.OrdensServico.Where(os => os.Ativo);

            var total = await query.CountAsync(ct);

            var ordensServico = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            var ordensServicoResponse = ordensServico.Select(os => MapToDTO(os)).ToList();

            return (ordensServicoResponse, total);
        }

        public async Task Update(OrdemServicoInputDTO ordemServico, CancellationToken ct)
        {
            var ordemServicoDbModel = await _appDbContext.OrdensServico
                .FirstOrDefaultAsync(os => os.Id == ordemServico.Id, ct);

            if (ordemServicoDbModel is null)
                throw new KeyNotFoundException("Ordem de Serviço não encontrada");

            ordemServicoDbModel.AlterarStatus(ordemServico.StatusOS);
            ordemServicoDbModel.AlterarObservacao(ordemServico.Observacao);
            ordemServicoDbModel.AlterarValores(
                ordemServico.ValorTotal,
                ordemServico.ValorDesconto,
                ordemServico.InicioExecucao,
                ordemServico.TerminoExecucao);
            ordemServicoDbModel.RastrearAlteracao(ordemServico.IdUsuarioAtualizacao!.Value, ordemServico.DataAtualizacao!.Value);

            _appDbContext.OrdensServico.Update(ordemServicoDbModel);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<List<OrdemServicoInputDTO>> GetByClienteId(Guid clienteId, CancellationToken ct)
        {
            var ordensServico = await _appDbContext.OrdensServico
                .Where(os => os.ClienteId == clienteId && os.Ativo)
                .AsNoTracking()
                .ToListAsync(ct);

            return ordensServico.Select(os => MapToDTO(os)).ToList();
        }

        public async Task<List<OrdemServicoInputDTO>> GetByStatus(string status, CancellationToken ct)
        {
            var ordensServico = await _appDbContext.OrdensServico
                .Where(os => os.StatusOS == status && os.Ativo)
                .AsNoTracking()
                .ToListAsync(ct);

            return ordensServico.Select(os => MapToDTO(os)).ToList();
        }

        private static OrdemServicoInputDTO MapToDTO(OrdemServicoDbModel dbModel)
        {
            return new OrdemServicoInputDTO
            {
                Id = dbModel.Id,
                ClienteId = dbModel.ClienteId,
                VeiculoId = dbModel.VeiculoId,
                StatusOS = dbModel.StatusOS,
                Observacao = dbModel.Observacao,
                ValorTotal = dbModel.ValorTotal,
                ValorDesconto = dbModel.ValorDesconto,
                InicioExecucao = dbModel.InicioExecucao,
                TerminoExecucao = dbModel.TerminoExecucao,
                IdUsuarioCriacao = dbModel.IdUsuarioCriacao,
                DataCriacao = dbModel.DataCriacao,
                IdUsuarioAtualizacao = dbModel.IdUsuarioAtualizacao,
                DataAtualizacao = dbModel.DataAtualizacao,
                Ativo = dbModel.Ativo
            };
        }
    }
}

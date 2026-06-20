using Application.Interfaces;
using Infra.Context;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.OrdemServicos.Input;
using Shared.DTOs.OrdemServicos.Shared;

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

            if(ordemServico.Pecas.Any())
                ordemServicoDbModel.Pecas = ordemServico.Pecas.Select(p => new OrdemServicoPecaDbModel(ordemServico.Id, p.PecaId, p.Quantidade, p.ValorUnitario)).ToList();

            if (ordemServico.Insumos.Any())
                ordemServicoDbModel.Insumos = ordemServico.Insumos.Select(i => new OrdemServicoInsumoDbModel(i.InsumoId, ordemServico.Id, i.Quantidade, i.CustoUnitario)).ToList();

            if (ordemServico.Servicos.Any())
                ordemServicoDbModel.Servicos = ordemServico.Servicos.Select(s => new OrdemServicoServicoDbModel(ordemServico.Id, s.ServicoId, s.ValorUnitario, s.Status, s.DataInicioExecucao, s.DataTerminoExecucao, s.Quantidade)).ToList();

            await _appDbContext.OrdensServico.AddAsync(ordemServicoDbModel, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<OrdemServicoInputDTO?> GetById(Guid id, CancellationToken ct)
        {
            var ordemServico = await _appDbContext.OrdensServico
                .Include(os => os.Insumos)
                .Include(os => os.Pecas)
                .Include(os => os.Servicos)
                .FirstOrDefaultAsync(os => os.Id == id && os.Ativo, ct);

            if (ordemServico is null)
                return null;

            return new OrdemServicoInputDTO
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
        }

        public async Task<(List<OrdemServicoInputDTO> ordensServico, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<OrdemServicoDbModel> query = _appDbContext.OrdensServico.Where(os => os.Ativo).Include(os => os.Insumos).Include(os => os.Pecas).Include(os => os.Servicos);

            var total = await query.CountAsync(ct);

            var ordensServico = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            var ordensServicoResponse = ordensServico.Select(os => new OrdemServicoInputDTO
            {
                Id = os.Id,
                ClienteId = os.ClienteId,
                VeiculoId = os.VeiculoId,
                StatusOS = os.StatusOS,
                Observacao = os.Observacao,
                ValorTotal = os.ValorTotal,
                ValorDesconto = os.ValorDesconto,
                InicioExecucao = os.InicioExecucao,
                TerminoExecucao = os.TerminoExecucao,
                IdUsuarioCriacao = os.IdUsuarioCriacao,
                DataCriacao = os.DataCriacao,
                IdUsuarioAtualizacao = os.IdUsuarioAtualizacao,
                DataAtualizacao = os.DataAtualizacao,
                Insumos = os.Insumos.Select(i => new OrdemServicoInsumoDTO
                {
                    CustoUnitario = i.CustoUnitario,
                    InsumoId = i.InsumoId,
                    Quantidade = i.Quantidade
                }).ToList(),
                Pecas = os.Pecas.Select(p => new OrdemServicoPecaDTO
                {
                    PecaId = p.PecaId,
                    Quantidade = p.Quantidade,
                    ValorUnitario = p.ValorUnitario
                }).ToList(),
                Servicos = os.Servicos.Select(s => new OrdemServicoServicoDTO
                {
                    DataInicioExecucao = s.DataInicioExecucao,
                    DataTerminoExecucao = s.DataTerminoExecucao,
                    Quantidade = s.Quantidade,
                    ServicoId = s.ServicoId,
                    Status = s.Status,
                    ValorUnitario = s.ValorUnitario
                }).ToList()
            }).ToList();

            return (ordensServicoResponse, total);
        }

        public async Task Update(OrdemServicoInputDTO ordemServico, CancellationToken ct)
        {
            var ordemServicoDbModel = await _appDbContext.OrdensServico
                .FirstOrDefaultAsync(os => os.Id == ordemServico.Id, ct);

            ordemServicoDbModel.Servicos = ordemServico.Servicos.Select(s => new OrdemServicoServicoDbModel(ordemServico.Id, s.ServicoId, s.ValorUnitario, s.Status, s.DataInicioExecucao, s.DataTerminoExecucao, s.Quantidade)).ToList();
            ordemServicoDbModel.Insumos = ordemServico.Insumos.Select(i => new OrdemServicoInsumoDbModel(i.InsumoId,ordemServico.Id, i.Quantidade, i.CustoUnitario)).ToList();
            ordemServicoDbModel.Pecas = ordemServico.Pecas.Select(p => new OrdemServicoPecaDbModel(ordemServico.Id, p.PecaId, p.Quantidade, p.ValorUnitario)).ToList();
            ordemServicoDbModel.ValorDesconto = ordemServico.ValorDesconto;
            ordemServicoDbModel.StatusOS = ordemServico.StatusOS;
            ordemServicoDbModel.Observacao = ordemServico.Observacao;
            ordemServicoDbModel.InicioExecucao = ordemServico.InicioExecucao;
            ordemServicoDbModel.TerminoExecucao = ordemServico.TerminoExecucao;


            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<List<OrdemServicoInputDTO>> GetByClienteId(Guid clienteId, CancellationToken ct)
        {
            var ordensServico = await _appDbContext.OrdensServico
                .Where(os => os.ClienteId == clienteId && os.Ativo)
                .Include(os => os.Insumos).Include(os => os.Pecas).Include(os => os.Servicos)
                .AsNoTracking()
                .ToListAsync(ct);

            return ordensServico.Select(os => new OrdemServicoInputDTO
            {
                Id = os.Id,
                ClienteId = os.ClienteId,
                VeiculoId = os.VeiculoId,
                StatusOS = os.StatusOS,
                Observacao = os.Observacao,
                ValorTotal = os.ValorTotal,
                ValorDesconto = os.ValorDesconto,
                InicioExecucao = os.InicioExecucao,
                TerminoExecucao = os.TerminoExecucao,
                IdUsuarioCriacao = os.IdUsuarioCriacao,
                DataCriacao = os.DataCriacao,
                IdUsuarioAtualizacao = os.IdUsuarioAtualizacao,
                DataAtualizacao = os.DataAtualizacao,
                Insumos = os.Insumos.Select(i => new OrdemServicoInsumoDTO
                {
                    CustoUnitario = i.CustoUnitario,
                    InsumoId = i.InsumoId,
                    Quantidade = i.Quantidade
                }).ToList(),
                Pecas = os.Pecas.Select(p => new OrdemServicoPecaDTO
                {
                    PecaId = p.PecaId,
                    Quantidade = p.Quantidade,
                    ValorUnitario = p.ValorUnitario
                }).ToList(),
                Servicos = os.Servicos.Select(s => new OrdemServicoServicoDTO
                {
                    DataInicioExecucao = s.DataInicioExecucao,
                    DataTerminoExecucao = s.DataTerminoExecucao,
                    Quantidade = s.Quantidade,
                    ServicoId = s.ServicoId,
                    Status = s.Status,
                    ValorUnitario = s.ValorUnitario
                }).ToList()
            }).ToList();
        }

        public async Task<List<OrdemServicoInputDTO>> GetByStatus(string status, CancellationToken ct)
        {
            var ordensServico = await _appDbContext.OrdensServico
                .Where(os => os.StatusOS == status && os.Ativo)
                .Include(os => os.Insumos).Include(os => os.Pecas).Include(os => os.Servicos)
                .AsNoTracking()
                .ToListAsync(ct);

            
            return ordensServico.Select(os => new OrdemServicoInputDTO
            {
                Id = os.Id,
                ClienteId = os.ClienteId,
                VeiculoId = os.VeiculoId,
                StatusOS = os.StatusOS,
                Observacao = os.Observacao,
                ValorTotal = os.ValorTotal,
                ValorDesconto = os.ValorDesconto,
                InicioExecucao = os.InicioExecucao,
                TerminoExecucao = os.TerminoExecucao,
                IdUsuarioCriacao = os.IdUsuarioCriacao,
                DataCriacao = os.DataCriacao,
                IdUsuarioAtualizacao = os.IdUsuarioAtualizacao,
                DataAtualizacao = os.DataAtualizacao,
                Insumos = os.Insumos.Select(i => new OrdemServicoInsumoDTO
                {
                    CustoUnitario = i.CustoUnitario,
                    InsumoId = i.InsumoId,
                    Quantidade = i.Quantidade
                }).ToList(),
                Pecas = os.Pecas.Select(p => new OrdemServicoPecaDTO
                {
                    PecaId = p.PecaId,
                    Quantidade = p.Quantidade,
                    ValorUnitario = p.ValorUnitario
                }).ToList(),
                Servicos = os.Servicos.Select(s => new OrdemServicoServicoDTO
                {
                    DataInicioExecucao = s.DataInicioExecucao,
                    DataTerminoExecucao = s.DataTerminoExecucao,
                    Quantidade = s.Quantidade,
                    ServicoId = s.ServicoId,
                    Status = s.Status,
                    ValorUnitario = s.ValorUnitario
                }).ToList()
            }).ToList();
        }
    }
}

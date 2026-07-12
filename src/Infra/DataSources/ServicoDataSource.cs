using Application.Interfaces;
using Infra.Context;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Servicos.Input;

namespace Infra.DataSources
{
    public class ServicoDataSource : IServicoDataSource
    {
        private readonly AppDbContext _appDbContext;

        public ServicoDataSource(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(ServicoInputDTO servico, CancellationToken ct)
        {
            var servicoDbModel = new ServicoDbModel(servico.Id, servico.Nome, servico.Descricao, servico.ValorUnitario, servico.TempoMedioExecucao, servico.IdUsuarioCriacao, servico.DataCriacao, servico.IdUsuarioAtualizacao, servico.DataAtualizacao, servico.Ativo);

            await _appDbContext.Servicos.AddAsync(servicoDbModel, ct);
            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task<ServicoInputDTO>? GetById(Guid id, CancellationToken ct)
        {
            IQueryable<ServicoDbModel> query = _appDbContext.Servicos.Where(s => s.Ativo);

            var servico = await _appDbContext.Servicos.SingleOrDefaultAsync(s => s.Id == id);

            if (servico is null)
                return null;

            var servicoResponse = new ServicoInputDTO
            {
                Id = servico.Id,
                Descricao = servico.Descricao,
                Nome = servico.Nome,
                TempoMedioExecucao = servico.TempoMedioExecucao,
                ValorUnitario = servico.ValorUnitario
            };

            return servicoResponse;
        }

        public async Task<ICollection<ServicoInputDTO>>? GetByIds(ICollection<Guid> ids, CancellationToken ct)
        {
            IQueryable<ServicoDbModel> query = _appDbContext.Servicos.Where(s => s.Ativo);

            var servicos = await _appDbContext.Servicos.Where(s => ids.Contains(s.Id)).ToListAsync(ct);

            if (servicos is null)
                return null;

            var servicoResponse = servicos.Select(s => new ServicoInputDTO
            {
                Id = s.Id,
                Descricao = s.Descricao,
                Nome = s.Nome,
                TempoMedioExecucao = s.TempoMedioExecucao,
                ValorUnitario = s.ValorUnitario
            }).ToList();

            return servicoResponse;
        }

        public async Task<(ICollection<ServicoInputDTO> servicos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            IQueryable<ServicoDbModel> query = _appDbContext.Servicos.Where(s => s.Ativo);

            var servicos = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .AsNoTracking()
                                      .ToListAsync(ct);

            var total = await query.CountAsync(ct);

            var servicoResponse = servicos.Select(s => new ServicoInputDTO
            {
                Id = s.Id,
                Descricao = s.Descricao,
                Nome = s.Nome,
                TempoMedioExecucao = s.TempoMedioExecucao,
                ValorUnitario = s.ValorUnitario
            }).ToList();

            return (servicoResponse, total);
        }

        public async Task Update(ServicoInputDTO servico, CancellationToken ct)
        {
            var servicoDbModel = await _appDbContext.Servicos.FirstOrDefaultAsync(s => s.Id == servico.Id, ct);

            if (servicoDbModel is null)
                throw new KeyNotFoundException("Serviço não encontrado");

            servicoDbModel.Id = servico.Id;
            servicoDbModel.Ativo = servico.Ativo;
            servicoDbModel.Descricao = servico.Descricao;
            servicoDbModel.DataAtualizacao = servico.DataAtualizacao;
            servicoDbModel.Nome = servico.Nome;
            servicoDbModel.ValorUnitario = servico.ValorUnitario;
            servicoDbModel.TempoMedioExecucao = servico.TempoMedioExecucao;
            servicoDbModel.IdUsuarioAtualizacao = servico.IdUsuarioAtualizacao;

            await _appDbContext.SaveChangesAsync(ct);
        }

        public async Task UpdateServicos(ICollection<ServicoInputDTO> servicos, CancellationToken ct)
        {
            var ids = servicos.Select(s => s.Id).ToList();
            var servicosModels = await _appDbContext.Servicos.Where(s => ids.Contains(s.Id)).ToListAsync(ct);

            if (servicosModels.Count() != servicos.Count())
                throw new KeyNotFoundException("Um ou mais serviços não encontrados!");

            ServicoInputDTO servico;

            foreach(var servicoDbModel in servicosModels)
            {
                servico = servicos.FirstOrDefault(s => s.Id == servicoDbModel.Id);

                servicoDbModel.Id = servico.Id;
                servicoDbModel.Ativo = servico.Ativo;
                servicoDbModel.Descricao = servico.Descricao;
                servicoDbModel.DataAtualizacao = servico.DataAtualizacao;
                servicoDbModel.Nome = servico.Nome;
                servicoDbModel.ValorUnitario = servico.ValorUnitario;
                servicoDbModel.TempoMedioExecucao = servico.TempoMedioExecucao;
                servicoDbModel.IdUsuarioAtualizacao = servico.IdUsuarioAtualizacao;
            }

            await _appDbContext.SaveChangesAsync(ct);
        }
    }
}

using Application.Interfaces;
using Domain.Entities;
using Shared.DTOs.Servicos.Input;

namespace Application.Gateways.Servicos
{
    public class ServicoGateway
    {
        private readonly IServicoDataSource _servicoDataSource;

        private ServicoGateway(IServicoDataSource servicoDataSource)
        {
            _servicoDataSource = servicoDataSource;
        }
        public static ServicoGateway Create(IServicoDataSource servicoDataSource)
        {
            return new ServicoGateway(servicoDataSource);
        }

        public async Task<(List<Servico> servicos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
        {
            var servicos = await _servicoDataSource.GetPaginated(page, pageSize, ct);

            var servicosEntity = servicos.servicos.Select(s => new Servico(s.Id, s.Nome, s.Descricao, s.ValorUnitario, s.TempoMedioExecucao)).ToList();

            return (servicosEntity, servicos.total);
        }
        public async Task<Servico>? GetById(Guid id, CancellationToken ct)
        {
            var servico = await _servicoDataSource.GetById(id, ct);

            if (servico is null)
                return null;

            var servicoEntity = new Servico(servico.Id, servico.Nome, servico.Descricao, servico.ValorUnitario, servico.TempoMedioExecucao);

            return servicoEntity;
        }
        public async Task<ICollection<Servico>>? GetByIds(ICollection<Guid> ids, CancellationToken ct)
        {
            var servicos = await _servicoDataSource.GetByIds(ids, ct);

            if (servicos is null)
                return null;

            var servicoEntity = servicos.Select(s =>  new Servico(s.Id, s.Nome, s.Descricao, s.ValorUnitario, s.TempoMedioExecucao)).ToList();

            return servicoEntity;
        }
        public async Task Create(Servico servico, CancellationToken ct)
        {
            var servicoInputDto = new ServicoInputDTO
            {
                Id = servico.Id,
                Ativo = servico.Ativo,
                DataAtualizacao = servico.DataAtualizacao,
                DataCriacao = servico.DataCriacao,
                Descricao = servico.Descricao,
                IdUsuarioAtualizacao = servico.IdUsuarioAtualizacao,
                IdUsuarioCriacao = servico.IdUsuarioCriacao,
                Nome = servico.Nome,
                TempoMedioExecucao = servico.TempoMedioExecucao,
                ValorUnitario = servico.ValorUnitario
            };

            await _servicoDataSource.Create(servicoInputDto, ct);
        }
        public async Task UpdateServicos(ICollection<Servico> servico, CancellationToken ct)
        {
            var servicosInputDto = servico.Select(s => new ServicoInputDTO
            {
                Id = s.Id,
                Ativo = s.Ativo,
                DataAtualizacao = s.DataAtualizacao,
                DataCriacao = s.DataCriacao,
                Descricao = s.Descricao,
                IdUsuarioAtualizacao = s.IdUsuarioAtualizacao,
                IdUsuarioCriacao = s.IdUsuarioCriacao,
                Nome = s.Nome,
                TempoMedioExecucao = s.TempoMedioExecucao,
                ValorUnitario = s.ValorUnitario
            }).ToList();

            await _servicoDataSource.UpdateServicos(servicosInputDto, ct);
        }
        public async Task Update(Servico servico, CancellationToken ct)
        {
            var servicoInputDto = new ServicoInputDTO
            {
                Id = servico.Id,
                Ativo = servico.Ativo,
                DataAtualizacao = servico.DataAtualizacao,
                DataCriacao = servico.DataCriacao,
                Descricao = servico.Descricao,
                IdUsuarioAtualizacao = servico.IdUsuarioAtualizacao,
                IdUsuarioCriacao = servico.IdUsuarioCriacao,
                Nome = servico.Nome,
                TempoMedioExecucao = servico.TempoMedioExecucao,
                ValorUnitario = servico.ValorUnitario
            };

            await _servicoDataSource.Update(servicoInputDto, ct);
        }
    }
}

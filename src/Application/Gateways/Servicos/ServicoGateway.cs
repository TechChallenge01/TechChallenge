using Application.Interfaces;
using Domain.Entities;

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
    }
}

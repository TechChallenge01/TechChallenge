using Domain.ValueObjects;

namespace Infra.DbModel
{
    public class ServicoDbModel
    {
        public ServicoDbModel(Guid id, string nome, string descricao, decimal valorUnitario, TimeSpan? tempoMedioExecucao, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao, bool ativo)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            ValorUnitario = valorUnitario;
            TempoMedioExecucao = tempoMedioExecucao;
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
            Ativo = ativo;
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal ValorUnitario { get; set; }
        public ICollection<OrdemServicoServico> OrdemServicoServicos { get; set; } = new List<OrdemServicoServico>();
        public TimeSpan? TempoMedioExecucao { get; set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}

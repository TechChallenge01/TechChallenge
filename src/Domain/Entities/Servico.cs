using Domain.BaseEntity;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Servico : Base
    {
        public Servico() { }

        public Servico(string nome, string descricao, decimal precoVenda, Guid idUsuarioCriacao, DateTime dataCriacao) : base(idUsuarioCriacao, dataCriacao, null, null)
        {

            ValidarDescricao(descricao);
            ValidarNome(nome);
            ValidarPrecoVenda(precoVenda);

            Id = Guid.NewGuid();

            Nome = nome;
            Descricao = descricao;
            ValorUnitario = precoVenda;
            UsuarioCriacaoId = idUsuarioCriacao;
            DataCriacao = dataCriacao;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public ICollection<OrdemServicoServico> OrdemServicoServicos { get; private set; } = new List<OrdemServicoServico>();
        public TimeSpan? TempoMedioExecucao { get; private set; }


        private void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do serviço é obrigatório.");
        }
        private void ValidarPrecoVenda(decimal precoVenda)
        {
            if (precoVenda <= 0)
                throw new ArgumentException("O preço de venda deve ser maior que zero.");
        }
        private void ValidarDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição do serviço é obrigatória.");
        }


        public void AlterarPrecoVenda(decimal novoPrecoVenda)
        {

            ValidarPrecoVenda(novoPrecoVenda);

            ValorUnitario = novoPrecoVenda;
        }
        public void AlterarDescricao(string novaDescricao)
        {
            ValidarDescricao(novaDescricao);

            Descricao = novaDescricao;
        }
        public void AlterarNome(string novoNome)
        {
            ValidarNome(novoNome);

            Nome = novoNome;
        }
        public void AtualizarTempoMedio(ICollection<TimeSpan> tempos)
        {
            if (tempos.Any())
            {
                TempoMedioExecucao = TimeSpan.FromTicks((long)tempos.Average(t => t.Ticks));
            }
        }
    }
}

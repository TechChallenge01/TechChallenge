using Domain.BaseEntity;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Peca : Base
    {
        public Peca(string name, string descricao, string marcaPeca, decimal precoVenda, Guid UsuarioCriacaoId, DateTime dataCriacao) : base(UsuarioCriacaoId, dataCriacao, null, null)
        {
            ValidarNome(name);
            ValidarPrecoVenda(precoVenda);
            ValidarMarcaPeca(marcaPeca);
            ValidaDescricao(descricao);

            Id = Guid.NewGuid();

            Nome = name;
            MarcaPeca = marcaPeca;
            ValorUnitario = precoVenda;
            Descricao = descricao;
        }

        protected Peca() { }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public string MarcaPeca { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public ICollection<OrdemServicoPeca> OrdemServicoPecas { get; private set; } = new List<OrdemServicoPeca>();

        private void ValidarNome(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome da peça é obrigatório.");
        }

        private void ValidaDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição da peça é obrigatória.");
        }

        private void ValidarMarcaPeca(string marcaPeca)
        {
            if (string.IsNullOrWhiteSpace(marcaPeca))
                throw new ArgumentException("A marca da peça é obrigatória.");
        }

        private void ValidarPrecoVenda(decimal precoVenda)
        {
            if (precoVenda <= 0)
                throw new ArgumentException("O preço de venda deve ser maior que zero.");
        }

        public void AlterarPrecoVenda(decimal novoPrecoVenda)
        {
            ValidarPrecoVenda(novoPrecoVenda);

            ValorUnitario = novoPrecoVenda;
        }

        public void AlterarMarcaPeca(string novaMarcaPeca)
        {
            ValidarMarcaPeca(novaMarcaPeca);

            MarcaPeca = novaMarcaPeca;
        }

        public void AlterarNome(string novoNome)
        {
            ValidarNome(novoNome);

            Nome = novoNome;
        }

        public void AlterarDescricao(string descricao)
        {
            ValidaDescricao(descricao);

            Descricao = descricao;
        }
    }
}

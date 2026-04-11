using Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class PecaEntity : BaseEntity
    {
        public PecaEntity(string name, string marcaPeca, decimal precoVenda, Guid UsuarioCriacaoId, DateTime dataCriacao) : base(UsuarioCriacaoId, dataCriacao, null, null)
        {
            ValidarNome(name);
            ValidarPrecoVenda(precoVenda);
            ValidarMarcaPeca(marcaPeca);

            Id = Guid.NewGuid();

            Name = name;
            MarcaPeca = marcaPeca;
            PrecoVenda = precoVenda;
        }

        protected PecaEntity() { }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string MarcaPeca { get; private set; }
        public decimal PrecoVenda { get; private set; }

        private void ValidarNome(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome da peça é obrigatório.");
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

        public void AlterarPrecoVenda(decimal novoPrecoVenda, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {
            ValidarPrecoVenda(novoPrecoVenda);

            PrecoVenda = novoPrecoVenda;

            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }

        public void AlterarMarcaPeca(string novaMarcaPeca, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {
            ValidarMarcaPeca(novaMarcaPeca);

            MarcaPeca = novaMarcaPeca;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }

        public void alterarNome(string novoNome, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {
            ValidarNome(novoNome);

            Name = novoNome;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }
    }
}

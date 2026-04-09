using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Domain.Entities
{
    public class MarcaVeiculo : Entity
    {
        public string Nome { get; private set; }

        public MarcaVeiculo(string nome, Guid idUsuarioCriacao)
        {
            ValidarMarca(nome);

            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = nome.Trim();
        }

        protected MarcaVeiculo() { }

        private void ValidarMarca(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da marca é obrigatório.");
        }
    }
}

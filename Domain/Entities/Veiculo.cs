using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace Domain.Entities
{
    public class Veiculo : Entity
    {
        public string Nome { get; private set; }
        public Guid IdMarcaVeiculo { get; private set; }
        public MarcaVeiculo Marca { get; private set; }
        public Guid IdCliente { get; private set; }
        public int Ano { get; private set; }
        public string Placa { get; private set; }
        public string Cor { get; private set; } 

        public Veiculo(string nome, Guid idMarcaVeiculo, Guid idCliente, int ano, string placa, string cor, Guid idUsuarioCriacao)
        {
            ValidaDados(nome, ano, placa);
            Nome = nome.Trim();
            IdMarcaVeiculo = idMarcaVeiculo;
            IdCliente = idCliente;
            Ano = ano;
            Placa = placa;
            Cor = cor;
            IdUsuarioCriacao = idUsuarioCriacao;
        }

        protected Veiculo() { }

        private void ValidaDados(string nome, int ano, string placa) 
        {
            if(string.IsNullOrWhiteSpace(nome)) throw new ArgumentNullException("O nome do veículo é obrigatório.");
            if (ano < 1900 || ano > DateTime.Now.Year + 1)
                throw new ArgumentException("Ano do veículo inválido.");
            if (string.IsNullOrWhiteSpace(placa)) throw new ArgumentNullException("A placa do veículo é obrigatória.");
        }
    }
}

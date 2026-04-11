using Domain.Agregates.Cliente;
using Domain.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace Domain.Aggregates.Cliente.ValueObjects
{
    public class Veiculo : BaseEntity
    {
        public Veiculo(string nome, Guid marcaVeiculoiD, Guid clienteId, int ano, string placa, string cor, Guid idUsuarioCriacao)
        {
            ValidaNome(nome);
            ValidaAno(ano);
            ValidaPlaca(placa);

            Nome = nome.Trim();
            MarcaVeiculoId = marcaVeiculoiD;
            ClienteId = clienteId;
            Ano = ano;
            Placa = placa;
            Cor = cor;
            IdUsuarioCriacao = idUsuarioCriacao;
        }

        protected Veiculo() { }

        public string Nome { get; private set; }
        public Guid MarcaVeiculoId { get; private set; }
        public Guid ClienteId { get; private set; }
        public int Ano { get; private set; }
        public string Placa { get; private set; }
        public string Cor { get; private set; }
        protected virtual MarcaVeiculo Marca { get; private set; }
        protected virtual ClienteEntity Cliente { get; private set; }

        private void ValidaNome(string nome) 
        {
            if(string.IsNullOrWhiteSpace(nome)) 
                throw new ArgumentNullException("O nome do veículo é obrigatório.");
        }
        private void ValidaAno(int ano) 
        {
            if (ano < 1900 || ano > DateTime.UtcNow.Year + 1)
                throw new ArgumentException("Ano do veículo inválido.");
        }
        private void ValidaPlaca(string placa) 
        {
            if (string.IsNullOrWhiteSpace(placa)) 
                throw new ArgumentNullException("A placa do veículo é obrigatória.");
        }
    }
}

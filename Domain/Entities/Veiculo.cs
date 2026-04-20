using Domain.Aggregates.ClienteAggregates;
using Domain.BaseEntity;

namespace Domain.Entities
{
    public class Veiculo : Base
    {
        public Veiculo(string modelo, Guid marcaVeiculoiD, Guid clienteId, int ano, string placa, string cor, Guid idUsuarioCriacao) : base(idUsuarioCriacao, DateTime.UtcNow, null, null)
        {
            ValidaModelo(modelo);
            ValidaAno(ano);
            ValidaPlaca(placa);

            Id = Guid.NewGuid();
            Modelo = modelo.Trim();
            MarcaVeiculoId = marcaVeiculoiD;
            ClienteId = clienteId;
            Ano = ano;
            Placa = placa;
            Cor = cor;
        }

        protected Veiculo() { }

        public Guid Id { get; private set; }
        public string Modelo { get; private set; }
        public Guid MarcaVeiculoId { get; private set; }
        public Guid ClienteId { get; private set; }
        public int Ano { get; private set; }
        public string Placa { get; private set; }
        public string Cor { get; private set; }
        protected virtual MarcaVeiculo Marca { get; private set; }
        protected virtual Cliente Cliente { get; private set; }
        public string NomeCliente => Cliente.Nome;
        public string NomeMarca => Marca.Nome;

        private void ValidaModelo(string modelo) 
        {
            if(string.IsNullOrWhiteSpace(modelo)) 
                throw new ArgumentException("O modelo do veículo é obrigatório.");
        }
        private void ValidaAno(int ano) 
        {
            if (ano < 1900 || ano > DateTime.UtcNow.Year + 1)
                throw new ArgumentException("Ano do veículo inválido.");
        }
        private void ValidaPlaca(string placa) 
        {
            if (string.IsNullOrWhiteSpace(placa)) 
                throw new ArgumentException("A placa do veículo é obrigatória.");
        }
    }
}

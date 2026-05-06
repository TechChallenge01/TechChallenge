using Domain.Aggregates.ClienteAggregates;
using Domain.Aggregates.OrdemServicoAggregates;
using Domain.BaseEntity;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Veiculo : Base
    {
        public Veiculo(string modelo, string marcaVeiculo, Guid clienteId, int ano, Placa placa, string cor, Guid idUsuarioCriacao) : base(idUsuarioCriacao, DateTime.UtcNow, null, null)
        {
            if (placa is null)
                throw new ArgumentException("a Placa não pode ser nulla");

            ValidaModelo(modelo);
            ValidaAno(ano);
            ValidaMarcaVeiculo(marcaVeiculo);
            ValidaPlaca(placa.ToString());
            ValidarCor(cor);


            Id = Guid.NewGuid();
            Modelo = modelo.Trim();
            MarcaVeiculo = marcaVeiculo;
            ClienteId = clienteId;
            Ano = ano;
            Placa = placa.ToString();
            Cor = cor;
            Ativo = true;
        }

        protected Veiculo() { }

        public Guid Id { get; private set; }
        public string Modelo { get; private set; }
        public string MarcaVeiculo { get; private set; }
        public Guid ClienteId { get; private set; }
        public int Ano { get; private set; }
        public string Placa { get; private set; }
        public string Cor { get; private set; }
        public ICollection<OrdemServico> OrdemServicos = new List<OrdemServico>();
        public virtual Cliente Cliente { get; private set; }        

        private void ValidaModelo(string modelo) 
        {
            if(string.IsNullOrWhiteSpace(modelo)) 
                throw new ArgumentException("O modelo do veículo é obrigatório.");
        }
        private void ValidaMarcaVeiculo(string marcaVeiculo) 
        {
            if(string.IsNullOrWhiteSpace(marcaVeiculo)) 
                throw new ArgumentException("A marca do veículo é obrigatória.");
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

        private void ValidarCor(string cor)
        {
            if (string.IsNullOrWhiteSpace(cor))
                throw new ArgumentException("A cor do veículo é obrigatória.");
        }

        public void AlterarModelo(string modelo)
        {
            ValidaModelo(modelo);
            Modelo = modelo.Trim();
        }

        public void AlterarMarcaVeiculo(string marcaVeiculo)
        {
            ValidaMarcaVeiculo(marcaVeiculo);
            MarcaVeiculo = marcaVeiculo;
        }

        public void AlterarAno(int ano)
        {
            ValidaAno(ano);
            Ano = ano;
        }

        public void AlterarCor(string cor)
        {
            ValidarCor(cor);
            Cor = cor;
        }

        public void AlterarCliente(Guid clienteId)
        {
            ClienteId = clienteId;
        }
    }
}

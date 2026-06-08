namespace Infra.DbModel
{
    public class VeiculoDbModel
    {
        public VeiculoDbModel(Guid id, string modelo, string marcaVeiculo, Guid clienteId, int ano, string placa, string cor, ClienteDbModel cliente, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao, bool ativo)
        {
            Id = id;
            Modelo = modelo;
            MarcaVeiculo = marcaVeiculo;
            ClienteId = clienteId;
            Ano = ano;
            Placa = placa;
            Cor = cor;
            Cliente = cliente;
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
            Ativo = ativo;
        }

        protected VeiculoDbModel() { }

        public Guid Id { get; private set; }
        public string Modelo { get; private set; }
        public string MarcaVeiculo { get; private set; }
        public Guid ClienteId { get; private set; }
        public int Ano { get; private set; }
        public string Placa { get; private set; }
        public string Cor { get; private set; }        
        public virtual ClienteDbModel Cliente { get; private set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}

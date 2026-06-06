using Infra.DataModel;

namespace Infra.DbModel
{
    public class VeiculoDbModel
    {
        public VeiculoDbModel(Guid id, string modelo, string marcaVeiculo, Guid clienteId, int ano, string placa, string cor, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao, bool ativo)
        {
            Id = id;
            Modelo = modelo;
            MarcaVeiculo = marcaVeiculo;
            ClienteId = clienteId;
            Ano = ano;
            Placa = placa;
            Cor = cor;
            UsuarioCriacaoId = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
            Ativo = ativo;
        }

        protected VeiculoDbModel() { }

        public Guid Id { get; set; }
        public string Modelo { get; set; }
        public string MarcaVeiculo { get; set; }
        public Guid ClienteId { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
        public string Cor { get; set; }        
        public virtual ClienteDbModel Cliente { get; set; }
        public Guid UsuarioCriacaoId { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}

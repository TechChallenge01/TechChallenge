namespace Infra.DbModel
{
    public class PecaDbModel
    {
        public PecaDbModel(Guid id, string nome, string descricao, string marcaPeca, decimal valorUnitario, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao, bool ativo)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            MarcaPeca = marcaPeca;
            ValorUnitario = valorUnitario;
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
            Ativo = ativo;
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string MarcaPeca { get; set; }
        public decimal ValorUnitario { get; set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
        public ICollection<OrdemServicoPecaDbModel> OrdemServicoPecas { get; set; } = new List<OrdemServicoPecaDbModel>();
    }
}

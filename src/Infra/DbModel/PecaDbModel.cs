namespace Infra.DbModel
{
    public class PecaDbModel
    {
        public PecaDbModel(Guid id, string nome, string descricao, string marcaPeca, decimal valorUnitario)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            MarcaPeca = marcaPeca;
            ValorUnitario = valorUnitario;
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string MarcaPeca { get; set; }
        public decimal ValorUnitario { get; set; }
        public ICollection<OrdemServicoPecaDbModel> OrdemServicoPecas { get; set; } = new List<OrdemServicoPecaDbModel> ();
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}

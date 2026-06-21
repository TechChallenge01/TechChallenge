namespace Infra.DbModel
{
    public class EstoqueHistoricoDbmodel
    {
        protected EstoqueHistoricoDbmodel() { }
        public EstoqueHistoricoDbmodel(Guid id, int quantidade, string observacao, string tipoMovimentacao, Guid estoqueId, Guid idUsuarioCriacao, DateTime dataCriacao)
        {
            Id = id;
            Quantidade = quantidade;
            Observacao = observacao;
            TipoMovimentacao = tipoMovimentacao;
            EstoqueId = estoqueId;
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
        }

        public Guid Id { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get;     set; } = string.Empty;
        public string TipoMovimentacao { get; set; }
        public Guid EstoqueId { get; set; }
        public virtual EstoqueDbModel Estoque { get; set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}

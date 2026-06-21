namespace Infra.DbModel
{
    public class EstoqueHistoricoDbmodel
    {
        protected EstoqueHistoricoDbmodel() { }
        public EstoqueHistoricoDbmodel(Guid id, int quantidade, string observacao, string tipoMovimentacao, Guid estoqueId)
        {
            Id = id;
            Quantidade = quantidade;
            Observacao = observacao;
            TipoMovimentacao = tipoMovimentacao;
            EstoqueId = estoqueId;            
        }

        public Guid Id { get; private set; }
        public int Quantidade { get; private set; }
        public string Observacao { get; private set; } = string.Empty;
        public string TipoMovimentacao { get; private set; }
        public Guid EstoqueId { get; private set; }
        public virtual EstoqueDbModel Estoque { get; private set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }

    }
}

namespace Infra.DbModel
{
    public class EstoqueDbModel
    {
        public EstoqueDbModel(Guid id, Guid? pecaId, Guid? insumoId, int quantidadeDisponivel, int quantidadeReservada, ICollection<EstoqueHistoricoDbmodel> historicos, PecaDbModel peca, InsumoDbModel insumo, bool ativo)
        {
            Id = id;
            PecaId = pecaId;
            InsumoId = insumoId;
            QuantidadeDisponivel = quantidadeDisponivel;
            QuantidadeReservada = quantidadeReservada;
            Historicos = historicos;
            Peca = peca;
            Insumo = insumo;
            Ativo = ativo;
        }

        protected EstoqueDbModel() { }

        public Guid Id { get; set; }
        public Guid? PecaId { get; set; }
        public Guid? InsumoId { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public int QuantidadeReservada { get; set; }
        public ICollection<EstoqueHistoricoDbmodel> Historicos { get; set; } = new List<EstoqueHistoricoDbmodel>();
        public virtual PecaDbModel Peca { get; set; }
        public virtual InsumoDbModel Insumo { get; set; }
        public bool Ativo { get; set; } = true;
    }
}

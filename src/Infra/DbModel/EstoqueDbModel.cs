namespace Infra.DbModel
{
    public class EstoqueDbModel
    {
        public EstoqueDbModel(Guid id, Guid? pecaId, Guid? insumoId, int quantidadeDisponivel, int quantidadeReservada, ICollection<EstoqueHistoricoDbmodel> historicos, PecaDbModel peca, InsumoDbModel insumo)
        {
            Id = id;
            PecaId = pecaId;
            InsumoId = insumoId;
            QuantidadeDisponivel = quantidadeDisponivel;
            QuantidadeReservada = quantidadeReservada;
            Historicos = historicos;
            Peca = peca;
            Insumo = insumo;
        }

        protected EstoqueDbModel() { }

        public Guid Id { get; private set; }
        public Guid? PecaId { get; private set; }
        public Guid? InsumoId { get; private set; }
        public int QuantidadeDisponivel { get; private set; }
        public int QuantidadeReservada { get; private set; }
        public ICollection<EstoqueHistoricoDbmodel> Historicos { get; private set; } = new List<EstoqueHistoricoDbmodel>();
        public virtual PecaDbModel Peca { get; private set; }
        public virtual InsumoDbModel Insumo { get; private set; }
    }
}

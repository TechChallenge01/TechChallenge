namespace Infra.DbModel
{
    public class OrdemServicoPecaDbModel
    {
        public OrdemServicoPecaDbModel() { }
        public OrdemServicoPecaDbModel(Guid ordemServicoId, Guid pecaId, int quantidade, decimal valorUnitario)
        {
            OrdemServicoId = ordemServicoId;
            PecaId = pecaId;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        public Guid OrdemServicoId { get; set; }
        public Guid PecaId { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public virtual PecaDbModel Peca { get; set; }
        public virtual OrdemServicoDbModel OrdemServico { get; set; }

    }
}

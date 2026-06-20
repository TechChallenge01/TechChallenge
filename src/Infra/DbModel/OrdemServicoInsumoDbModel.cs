namespace Infra.DbModel
{
    public class OrdemServicoInsumoDbModel
    {
        public OrdemServicoInsumoDbModel(Guid insumoId, Guid ordemServicoId, int quantidade, decimal custoUnitario)
        {
            InsumoId = insumoId;
            OrdemServicoId = ordemServicoId;
            Quantidade = quantidade;
            CustoUnitario = custoUnitario;
        }

        public Guid InsumoId { get; set; }
        public Guid OrdemServicoId { get; set; }
        public int Quantidade { get; set; }
        public decimal CustoUnitario { get; set; }
        public virtual InsumoDbModel Insumo { get; set; }
        public virtual OrdemServicoDbModel OrdemServico { get; set; }
    }
}

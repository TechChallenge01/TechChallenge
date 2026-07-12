namespace Domain.ValueObjects
{
    public class OrdemServicoInsumo
    {
        public OrdemServicoInsumo(Guid ordemServicoId, Guid insumoId, int quantidade, decimal custoUnitario)
        {
            ValidarQuantidade(quantidade);
            ValidarCustoUnitario(custoUnitario);

            InsumoId = insumoId;
            Quantidade = quantidade;
            CustoUnitario = custoUnitario;
            OrdemServicoId = ordemServicoId;
        }

        protected OrdemServicoInsumo() { }


        public Guid InsumoId { get; private set; }
        public Guid OrdemServicoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal CustoUnitario { get; private set; }
        public decimal ValorTotal => Quantidade * CustoUnitario;
        public virtual Insumo Insumo { get; private set; }
        private void ValidarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero.");
        }

        private void ValidarCustoUnitario(decimal custoUnitario)
        {
            if (custoUnitario <= 0)
                throw new ArgumentException("Custo unitário deve ser positivo.");
        }
    }
}

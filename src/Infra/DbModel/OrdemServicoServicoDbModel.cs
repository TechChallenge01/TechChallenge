namespace Infra.DbModel
{
    public class OrdemServicoServicoDbModel
    {
        public OrdemServicoServicoDbModel(Guid ordemServicoId, Guid servicoId, decimal valorUnitario, string status, DateTime? dataInicioExecucao, DateTime? dataTerminoExecucao, int quantidade)
        {
            OrdemServicoId = ordemServicoId;
            ServicoId = servicoId;
            ValorUnitario = valorUnitario;
            Status = status;
            DataInicioExecucao = dataInicioExecucao;
            DataTerminoExecucao = dataTerminoExecucao;
            Quantidade = quantidade;
        }

        public Guid OrdemServicoId { get; set; }
        public Guid ServicoId { get; set; }
        public decimal ValorUnitario { get; set; }
        public string Status { get; set; }
        public DateTime? DataInicioExecucao { get; set; }
        public DateTime? DataTerminoExecucao { get; set; }
        public int Quantidade { get; set; }
        public virtual ServicoDbModel Servico { get; set; }
        public virtual OrdemServicoDbModel OrdemServico { get; set; }
    }
}

namespace Shared.DTOs.OrdemServicos.Input
{
    public class OrdemServicoInputDTO
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid VeiculoId { get; set; }
        public string StatusOS { get; set; }
        public string? Observacao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDesconto { get; set; }
        public DateTime? InicioExecucao { get; set; }
        public DateTime? TerminoExecucao { get; set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}

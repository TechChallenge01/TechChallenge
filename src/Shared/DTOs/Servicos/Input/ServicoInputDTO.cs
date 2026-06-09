namespace Shared.DTOs.Servicos.Input
{
    public record ServicoInputDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public decimal ValorUnitario { get; init; }
        public TimeSpan? TempoMedioExecucao { get; init; }
    }
}

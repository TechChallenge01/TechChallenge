namespace Shared.DTOs.Servicos.Output
{
    public class ServicoOutputDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public decimal PrecoVenda { get; init; }
        public TimeSpan? TempoMedioExecucao { get; init; }
    }
}

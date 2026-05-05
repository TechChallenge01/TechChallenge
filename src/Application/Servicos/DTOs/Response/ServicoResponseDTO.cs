namespace Application.Servicos.DTOs.Response;

public record ServicoResponseDTO
{
    public Guid Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public decimal PrecoVenda { get; init; }
    public TimeSpan? TempoMedioExecucao { get; init; }
}

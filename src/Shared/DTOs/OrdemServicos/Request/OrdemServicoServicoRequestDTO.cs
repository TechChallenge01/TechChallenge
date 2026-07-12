namespace Shared.DTOs.OrdemServicos.Shared;

public record OrdemServicoServicoRequestDTO
{
    public Guid ServicoId { get; init; }
    public int Quantidade { get; init; }
    public decimal ValorUnitario { get; init; }
    public DateTime? DataInicioExecucao { get; init; }
    public DateTime? DataTerminoExecucao { get; init; }
    public string Status { get; init; }
}

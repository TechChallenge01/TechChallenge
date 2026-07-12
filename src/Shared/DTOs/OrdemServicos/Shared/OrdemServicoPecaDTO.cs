namespace Shared.DTOs.OrdemServicos.Shared;

public record OrdemServicoPecaDTO
{
    public Guid PecaId { get; init; }
    public int Quantidade { get; init; }
    public decimal ValorUnitario { get; init; }
}

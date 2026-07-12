namespace Shared.DTOs.OrdemServicos.Shared;

public record OrdemServicoPecaRequestDTO
{
    public Guid PecaId { get; init; }
    public int Quantidade { get; init; }
}

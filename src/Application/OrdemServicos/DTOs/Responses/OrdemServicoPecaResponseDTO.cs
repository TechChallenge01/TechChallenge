namespace Application.OrdemServicos.DTOs.Responses;

public record OrdemServicoPecaResponseDTO
{
    public Guid PecaId { get; init; }
    public int Quantidade { get; init; }
    public decimal ValorUnitario { get; init; }
    public decimal ValorTotal {  get; init; }
}

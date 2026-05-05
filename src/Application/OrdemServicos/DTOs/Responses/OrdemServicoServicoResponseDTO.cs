namespace Application.OrdemServicos.DTOs.Responses;

public record OrdemServicoServicoResponseDTO
{
    public Guid ServicoId { get; init; }
    public string NomeServico { get; init; }
    public string DescricaoServico { get; init; }
    public decimal ValorUnitario { get; init; }
    public int Quantidade { get; init; }
    public decimal ValorTotal { get; init; }
    public string StatusOS { get; init; }
}

namespace Application.OrdemServicos.DTOs.Responses;

public record OrdemServicoServicoResponseDTO
{
    public Guid ServicoId { get; set; }
    public string NomeServico { get; set; }
    public string DescricaoServico { get; set; }
    public decimal ValorUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public string StatusOS { get; set; }
}

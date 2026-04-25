namespace Application.OrdemServicos.DTOs.Responses;

public record OrdemServicoPecaResponseDTO
{
    public Guid PecaId { get; set; }
    public string NomePeca { get; set; }
    public string DescricaoPeca { get;  set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal {  get; set; }
}

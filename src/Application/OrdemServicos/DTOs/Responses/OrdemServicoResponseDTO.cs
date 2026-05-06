namespace Application.OrdemServicos.DTOs.Responses;

public record OrdemServicoResponseDTO
{
    public Guid Id { get; init; }  
    public Guid ClienteId {  get; init; }
    public Guid VeiculoId {  get; init; }
    public string StatusOS { get;  init; }
    public string? Observacao { get;  init; }
    public decimal ValorTotal { get;  init; }
    public decimal ValorDesconto { get;  init; } = 0;
    public TimeSpan TempoExecucao { get; init; }
    public ICollection<OrdemServicoPecaResponseDTO>? Pecas { get; init; }
    public ICollection<OrdemServicoServicoResponseDTO>? Servicos { get; init; }
    public ICollection<OrdemServicoInsumoResponseDTO>? Insumos { get; init; }
}

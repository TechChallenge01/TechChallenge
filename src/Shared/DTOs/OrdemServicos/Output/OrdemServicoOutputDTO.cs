using Shared.DTOs.OrdemServicos.Shared;

namespace Shared.DTOs.OrdemServicos.Output;

public record OrdemServicoOutputDTO
{
    public Guid Id { get; init; }  
    public Guid ClienteId {  get; init; }
    public Guid VeiculoId {  get; init; }
    public string StatusOS { get;  init; }
    public string? Observacao { get;  init; }
    public decimal ValorTotal { get;  init; }
    public decimal ValorDesconto { get;  init; } = 0;
    public TimeSpan TempoExecucao { get; init; }
    public ICollection<OrdemServicoPecaDTO>? Pecas { get; init; }
    public ICollection<OrdemServicoServicoDTO>? Servicos { get; init; }
    public ICollection<OrdemServicoInsumoDTO>? Insumos { get; init; }
}

namespace Application.OrdemServicos.DTOs.Responses;

public record OrdemServicoResponseDTO
{
    public string NomeCliente { get;  set; }
    public string ModeloVeiculo { get;  set; }
    public string PlacaVeiculo { get;  set; }
    public string MarcaVeiculo { get;  set; }   
    public string StatusOS { get;  set; }
    public string? Observacao { get;  set; }
    public decimal ValorTotal { get;  set; }
    public decimal ValorDesconto { get;  set; } = 0;
    public TimeSpan TempoExecucao { get; set; }
    public ICollection<OrdemServicoPecaResponseDTO>? Pecas { get; set; }
    public ICollection<OrdemServicoServicoResponseDTO>? Servicos { get; set; }
    public ICollection<OrdemServicoInsumoResponseDTO>? Insumos { get; set; }
}

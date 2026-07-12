namespace Shared.DTOs.Pecas.Output;

public record PecaOutputDTO
{
    public Guid Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public string MarcaPeca { get; init; }
    public decimal ValorUnitario { get; init; }
}

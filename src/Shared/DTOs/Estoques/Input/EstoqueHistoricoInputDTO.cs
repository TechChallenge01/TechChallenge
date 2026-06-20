namespace Shared.DTOs.Estoques.Input;

public class EstoqueHistoricoInputDTO
{
    public Guid Id { get; init; }
    public int Quantidade { get; init; }
    public string Observacao { get; init; }
    public string TipoMovimentacao { get; init; }
    public Guid EstoqueId { get; init; }
    public Guid IdUsuarioCriacao { get; init; }
    public DateTime DataCriacao { get; init; }
}

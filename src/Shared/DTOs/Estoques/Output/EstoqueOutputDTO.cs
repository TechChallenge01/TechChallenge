namespace Shared.DTOs.Estoques.Output;
public class EstoqueOutputDTO
{
    public Guid Id { get; init; }
    public Guid? PecaId { get; init; }
    public Guid? InsumoId { get; init; }
    public int QuantidadeDisponivel { get; init; }
    public int QuantidadeReservada { get; init; }
    public int QuantidadeTotal { get; init; }

}

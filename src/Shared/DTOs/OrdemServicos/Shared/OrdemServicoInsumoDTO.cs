namespace Shared.DTOs.OrdemServicos.Shared
{
    public record OrdemServicoInsumoDTO
    {
        public Guid InsumoId { get; init; }
        public int Quantidade { get; init; }
        public decimal CustoUnitario { get; init; }
    }
}

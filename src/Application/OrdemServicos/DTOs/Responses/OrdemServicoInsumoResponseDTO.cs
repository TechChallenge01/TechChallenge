namespace Application.OrdemServicos.DTOs.Responses
{
    public record OrdemServicoInsumoResponseDTO
    {
        public Guid InsumoId { get; init; }
        public int Quantidade { get; init; }
        public decimal CustoUnitario { get; init; }
        public decimal CustoTotal { get; init; }
    }
}

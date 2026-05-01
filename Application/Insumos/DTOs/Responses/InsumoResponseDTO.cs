namespace Application.Insumos.DTOs.Responses
{
    public class InsumoResponseDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public decimal CustoUnitario { get; init; }
    }
}

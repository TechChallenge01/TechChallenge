namespace Application.Estoques.DTOs.Responses
{
    public record EstoqueHistoricoResponseDTO
    {
        public int Quantidade { get; init; }
        public string Observacao { get; init; } = string.Empty;
        public string TipoMovimentacao { get; init; }
        public Guid EstoqueId { get; init; }
    }
}

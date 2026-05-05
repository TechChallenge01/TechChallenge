namespace Application.Pecas.DTOs.Responses
{
    public record PecaResponseDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public string MarcaPeca { get; init; }
        public decimal PrecoVenda { get; init; }
    }
}

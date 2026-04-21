namespace Application.Pecas.DTOs.Responses
{
    public record PecaResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string MarcaPeca { get; set; }
        public decimal PrecoVenda { get; set; }
    }
}

namespace Application.Estoques.DTOs.Responses
{
    public record EstoqueResponseDTO
    {
        public Guid Id { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public int QuantidadeReservada { get; set; }
        public int QuantidadeTotal { get; set; }
        public string NomePeca { get; set; }
        public string NomeInsumo { get; set; }
    }
}

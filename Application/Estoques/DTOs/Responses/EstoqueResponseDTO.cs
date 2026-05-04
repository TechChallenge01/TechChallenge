namespace Application.Estoques.DTOs.Responses
{
    public record EstoqueResponseDTO
    {
        public Guid Id { get; init; }
        public int QuantidadeDisponivel { get; init; }
        public int QuantidadeReservada { get; init; }
        public int QuantidadeTotal { get; init; }
        public Guid? PecaId { get; init;  }
        public Guid? InsumoId { get; init;  }
    }
}

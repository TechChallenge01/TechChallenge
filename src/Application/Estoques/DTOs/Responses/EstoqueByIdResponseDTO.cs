using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Estoques.DTOs.Responses
{
    public record EstoqueByIdResponseDTO
    {
        public Guid Id { get; init; }
        public int QuantidadeDisponivel { get; init; }
        public int QuantidadeReservada { get; init; }
        public int QuantidadeTotal { get; init; }
        public Guid? PecaId { get; init; }
        public Guid? InsumoId { get; init; }
        public ICollection<EstoqueHistoricoResponseDTO>? Historico { get; init; }
    }
}

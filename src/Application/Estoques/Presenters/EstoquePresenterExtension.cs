using Application.Estoques.DTOs.Responses;
using Domain.Aggregates.EstoqueAggregates;

namespace Application.Estoques.Presenters
{
    public static class EstoquePresenterExtension
    {
        public static EstoqueByIdResponseDTO ToDTOId(this Estoque estoque)
        {
            return new EstoqueByIdResponseDTO
            {
                Id = estoque.Id,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                QuantidadeReservada = estoque.QuantidadeReservada,
                QuantidadeTotal = estoque.QuantidadeTotal,
                PecaId = estoque.PecaId,
                InsumoId = estoque.InsumoId,
                Historico = estoque.Historicos.Select(h => new EstoqueHistoricoResponseDTO
                {
                    EstoqueId = estoque.Id,
                    Observacao = h.Observacao,
                    Quantidade = h.Quantidade,
                    TipoMovimentacao = h.TipoMovimentacao.ToString()
                }).ToList()
            };
        }
        public static EstoqueResponseDTO ToDTO(this Estoque estoque)
        {
            return new EstoqueResponseDTO
            {
                Id = estoque.Id,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                QuantidadeReservada = estoque.QuantidadeReservada,
                QuantidadeTotal = estoque.QuantidadeTotal,
                PecaId = estoque.PecaId,
                InsumoId = estoque.InsumoId,
            };
        }

        public static ICollection<EstoqueResponseDTO> ToDTOList(this ICollection<Estoque> estoques)
        {
            return estoques.Select(e => ToDTO(e)).ToList();
        }   
    }
}

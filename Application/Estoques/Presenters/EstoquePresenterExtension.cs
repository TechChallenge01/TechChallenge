using Application.Estoques.DTOs.Responses;
using Domain.Aggregates.EstoqueAggregates;

namespace Application.Estoques.Presenters
{
    public static class EstoquePresenterExtension
    {
        public static EstoqueResponseDTO ToDTO(this Estoque estoque)
        {
            return new EstoqueResponseDTO
            {
                Id = estoque.Id,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                QuantidadeReservada = estoque.QuantidadeReservada,
                QuantidadeTotal = estoque.QuantidadeTotal,
                NomePeca = estoque.NomePeca,
                NomeInsumo = estoque.NomeInsumo
            };
        }

        public static ICollection<EstoqueResponseDTO> ToDTOList(this ICollection<Estoque> estoques)
        {
            return estoques.Select(e => ToDTO(e)).ToList();
        }   
    }
}

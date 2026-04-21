using Application.Pecas.DTOs.Responses;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Pecas.Presenters
{
    public static class PecaPresenterExtension
    {
        public static PecaResponseDTO ToDto(this Peca peca)
        {
            return new PecaResponseDTO
            {
                Id = peca.Id,
                Nome = peca.Nome,
                Descricao = peca.Descricao,
                MarcaPeca = peca.MarcaPeca,
                PrecoVenda = peca.PrecoVenda
            };
        }
        public static ICollection<PecaResponseDTO> ToDtoList(this ICollection<Peca> pecas)
        {
            return pecas.Select(p => ToDto(p)).ToList();
        }
    }
}

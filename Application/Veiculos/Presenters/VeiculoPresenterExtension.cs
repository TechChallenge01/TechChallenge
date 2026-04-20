using Application.Veiculos.DTOs.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Veiculos.Presenters
{
    public static class VeiculoPresenterExtension
    {
        public static VeiculoResponseDTO ToDto(this Veiculo servico)
        {
            return new VeiculoResponseDTO
            {
                Id = servico.Id,
                Marca = servico.NomeMarca,
                Modelo = servico.Modelo,
                Ano = servico.Ano,
                Placa = servico.Placa,
                Cor = servico.Cor,
                NomeCliente = servico.NomeCliente
            };
        }

        public static List<VeiculoResponseDTO> ToDtoList(this IEnumerable<Veiculo> veiculos)
        {
            return veiculos.Select(v => ToDto(v)).ToList();
        }
    }
}

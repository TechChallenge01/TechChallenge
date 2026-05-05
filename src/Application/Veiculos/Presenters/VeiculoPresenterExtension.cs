using Application.Veiculos.DTOs.Response;
using Domain.Entities;

namespace Application.Veiculos.Presenters
{
    public static class VeiculoPresenterExtension
    {
        public static VeiculoResponseDTO ToDto(this Veiculo veiculos)
        {
            return new VeiculoResponseDTO
            {
                Id = veiculos.Id,
                MarcaVeiculo = veiculos.MarcaVeiculo,
                Modelo = veiculos.Modelo,
                Ano = veiculos.Ano,
                Placa = veiculos.Placa,
                Cor = veiculos.Cor
            };
        }

        public static List<VeiculoResponseDTO> ToDtoList(this IEnumerable<Veiculo> veiculos)
        {
            return veiculos.Select(v => ToDto(v)).ToList();
        }
    }
}

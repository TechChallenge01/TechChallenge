using Application.Veiculos.DTOs.Response;
using Domain.Entities;

namespace Application.Veiculos.Presenters
{
    public static class VeiculoPresenterExtension
    {
        public static VeiculoResponseDTO ToDto(this Veiculo servico)
        {
            return new VeiculoResponseDTO
            {
                Id = servico.Id,
                MarcaVeiculo = servico.MarcaVeiculo,
                Modelo = servico.Modelo,
                Ano = servico.Ano,
                Placa = servico.ValorPlaca,
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

using Application.Insumos.DTOs.Responses;

namespace Application.Insumos.Presenters
{
    public static class InsumoPresenterExtension
    {
        public static InsumoResponseDTO ToDto(this Insumo insumo)
        {
            return new InsumoResponseDTO
            {
                Id = insumo.Id,
                Nome = insumo.Nome,
                Descricao = insumo.Descricao,
                CustoUnitario = insumo.CustoUnitario
            };
        }

        public static ICollection<InsumoResponseDTO> ToDtoList(this ICollection<Insumo> insumos)
        {
            return insumos.Select(insumo => insumo.ToDto()).ToList();
        }
    }
}

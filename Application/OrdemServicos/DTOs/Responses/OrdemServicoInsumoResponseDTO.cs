using System;
using System.Collections.Generic;
using System.Text;

namespace Application.OrdemServicos.DTOs.Responses
{
    public record OrdemServicoInsumoResponseDTO
    {
        public Guid InsumoId { get; set; }
        public string NomeInsumo { get; set; }
        public string DescricaoInsumo { get; set; }
        public int Quantidade { get; set; }
        public decimal CustoUnitario { get; set; }
        public decimal CustoTotal { get; set; }
    }
}

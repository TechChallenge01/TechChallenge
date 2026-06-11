using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Insumos.Output;

public record InsumoOutputDTO
{
    public Guid Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public decimal CustoUnitario { get; init; } 
}

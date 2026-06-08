using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Insumo.Input;

public record InsumoInputDTO
{
    public Guid Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public decimal CustoUnitario { get; init; }
    public Guid IdUsuarioCriacao { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid? IdUsuarioAtualizacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; } = true;
}

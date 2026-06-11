using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Estoques.Input;

public class EstoqueInputDTO
{
    public Guid Id { get; init; }
    public Guid? PecaId { get; init; }
    public Guid? InsumoId { get; init; }
    public int QuantidadeDisponivel { get; init; }
    public int QuantidadeReservada { get; init; }
    public Guid IdUsuarioCriacao { get; init; }
    public DateTime DataCriacao { get; init; }
    public Guid? IdUsuarioAtualizacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; } = true;
    public List<EstoqueHistoricoInputDTO> Historicos { get; set; } = new();
}

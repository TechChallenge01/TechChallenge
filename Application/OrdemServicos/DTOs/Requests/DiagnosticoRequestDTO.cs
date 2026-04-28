using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.OrdemServicos.DTOs.Requests;

public record DiagnosticoRequestDTO
{
    public ICollection<OrdemServicoPecaRequestDTO>? Pecas { get; init; }
    public ICollection<OrdemServicoServicoRequestDTO>? Servicos { get; init; }

    [MaxLength(500, ErrorMessage = "O campo Observação deve conter no máximo 500 caracteres.")]
    public string? Observacao { get; init; }
}

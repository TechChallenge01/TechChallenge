using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Servicos.Requests;

public class ServicoRequestDTO
{
    [Required]
    public string Nome { get; init; }
    [Required]
    public string Descricao { get; init; }
    [Required]
    public decimal PrecoVenda { get; init; }
}

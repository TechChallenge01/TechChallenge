using System.ComponentModel.DataAnnotations;

namespace Application.Servicos.DTOs.Requests;

public class ServicoRequestDTO
{
    [Required]
    public string Nome { get; private set; }
    [Required]
    public string Descricao { get; private set; }
    [Required]
    public decimal PrecoVenda { get; private set; }
}

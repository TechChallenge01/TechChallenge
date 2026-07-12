namespace Shared.DTOs.Pecas.Input;

public record PecaInputDTO
{
    public Guid Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public string MarcaPeca { get; init; }
    public decimal ValorUnitario { get; init; }
    public Guid IdUsuarioCriacao { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid? IdUsuarioAtualizacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; } = true;
}
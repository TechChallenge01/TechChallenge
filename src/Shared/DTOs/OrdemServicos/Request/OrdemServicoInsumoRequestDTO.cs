namespace Shared.DTOs.OrdemServicos.Shared
{
    public record OrdemServicoInsumoRequestDTO
    {
        public Guid InsumoId { get; init; }
        public int Quantidade { get; init; }
    }
}

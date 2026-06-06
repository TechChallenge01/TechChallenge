namespace Shared.DTOs.Veiculos.Output
{
    public record VeiculoOutputDTO
    {
        public Guid Id { get; set; }
        public string Modelo { get; init; }
        public string MarcaVeiculo { get; init; }
        public Guid ClienteId { get; init; }
        public int Ano { get; init; }
        public string Placa { get; init; }
        public string Cor { get; init; }
        public Guid UsuarioCriacaoId { get; init; }
    }
}

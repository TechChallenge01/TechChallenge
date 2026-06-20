namespace Shared.DTOs.Veiculos.Input
{
    public record VeiculoInputDTO
    {
        public Guid Id { get; set; }
        public string Modelo { get; init; }
        public string MarcaVeiculo { get; init; }
        public Guid ClienteId { get; init; }
        public int Ano { get; init; }
        public string Placa { get; init; }
        public string Cor { get; init; }
        public Guid UsuarioCriacaoId { get; init; }
        public DateTime DataCriacao { get; init; }
        public Guid? UsuarioAlteracaoId { get; init; }
        public DateTime? DataAlteracao { get; init; }
        public bool Ativo { get; init; }
    }
}

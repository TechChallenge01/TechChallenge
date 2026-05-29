using Shared.DTOs.Cliente.Shared;

namespace Shared.DTOs.Cliente.Output
{
    public record ClienteOutputDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string? Cpf { get; init; }
        public string? Cnpj { get; init; }
        public string Email { get; init; }
        public TelefoneDTO Telefone { get; init; }
        public EnderecoDTO Endereco { get; init; }
        public List<Guid> Veiculos { get; init; }
    }
}

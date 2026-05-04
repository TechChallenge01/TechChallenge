using Application.Clientes.DTOs.Shared;

namespace Application.Clientes.DTOs.Responses
{
    public record ClienteResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Cnpj { get; set; }
        public string Email { get; set; }
        public TelefoneDTO Telefone { get; set; }
        public EnderecoDTO Endereco { get; set; }
    }
}

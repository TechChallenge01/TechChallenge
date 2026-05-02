using Application.Clientes.DTOs.Shared;

namespace Application.Clientes.DTOs.Responses
{
    public record ClienteResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Cnpj { get; set; }
        public ICollection<string> Emails { get; set; }
        public ICollection<TelefoneDTO> Telefones { get; set; }
        public ICollection<EnderecoDTO> Enderecos { get; set; }
    }
}

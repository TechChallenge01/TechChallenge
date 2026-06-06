using Shared.DTOs.Clientes.Shared;

namespace Shared.DTOs.Clientes.Input
{
    public record ClienteInputDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string? Cpf { get; init; }
        public string? Cnpj { get; init; }
        public string Email { get; init; }
        public TelefoneDTO Telefone { get; init; }
        public EnderecoDTO Endereco { get; init; }
        public ICollection<Guid> Veiculos { get; init; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;
    }
}

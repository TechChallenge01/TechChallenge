namespace Shared.DTOs.Usuarios.Input
{
    public record UsuarioInputDTO
    {
        public Guid Id { get; init; }
        public string Nome { get; init; }
        public string Email { get; init; }
        public string SenhaHash { get; init; }
        public string Perfil { get; init; }
        public Guid IdUsuarioCriacao { get; init; }
        public DateTime DataCriacao { get; init; }
        public Guid? IdUsuarioAtualizacao { get; init; }
        public DateTime? DataAtualizacao { get; init; }
        public bool Ativo { get; init; } = true;
    }
}

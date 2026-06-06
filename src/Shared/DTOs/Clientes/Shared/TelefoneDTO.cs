namespace Shared.DTOs.Clientes.Shared
{
    public record TelefoneDTO
    {
        public string DDD { get; init; }
        public string DDI { get; init; }
        public string Numero { get; init; }
    }
}

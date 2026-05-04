namespace Application.Clientes.DTOs.Shared
{
    public record TelefoneDTO
    {
        public string DDD { get;  set; }
        public string DDI { get;  set; }
        public string Numero { get;  set; }
    }
}

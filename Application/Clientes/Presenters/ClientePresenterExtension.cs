using Application.Clientes.DTOs.Responses;
using Application.Clientes.DTOs.Shared;

namespace Application.Clientes.Presenters
{
    public static class ClientePresenterExtension
    {
        public static ClienteResponseDTO ToDto(this Domain.Aggregates.ClienteAggregates.Cliente cliente)
        {
            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                cpf = cliente.Cpf?.ToString(),
                cnpj = cliente.Cnpj?.ToString(),
                Emails = cliente.Emails != null ? cliente.Emails.Select(e => e.EnderecoEmail).ToList() : new List<string>(),

                Telefones = cliente.Telefones != null ? cliente.Telefones.Select(t => new TelefoneDTO
                {
                    DDD = t.DDD,
                    DDI = t.DDI,
                    Numero = t.Numero,
                    Tipo = t.TipoTelefone.ToString()
                }).ToList() : new List<TelefoneDTO>(),

                Enderecos = cliente.Enderecos != null ? cliente.Enderecos.Select(e => new EnderecoDTO
                {
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Cep = e.Cep
                }).ToList() : new List<EnderecoDTO>()
            };
        }

        public static List<ClienteResponseDTO> ToListDTO(this IEnumerable<Domain.Aggregates.ClienteAggregates.Cliente> clientes)
        {
            return clientes.Select(c => c.ToDto()).ToList();
        }
    }
}

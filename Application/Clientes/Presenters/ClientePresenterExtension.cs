using Application.Clientes.DTOs.Responses;
using Application.Clientes.DTOs.Shared;
using Domain.Aggregates.ClienteAggregates;

namespace Application.Clientes.Presenters
{
    public static class ClientePresenterExtension
    {
        public static ClienteResponseDTO ToDto(this Cliente cliente)
        {
            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Cpf = cliente.Cpf?.Valor.ToString(),
                Cnpj = cliente.Cnpj?.Valor.ToString(),
                Email = cliente.Email.EnderecoEmail,

                Telefone = new TelefoneDTO
                {
                    DDD = cliente.Telefone.DDD,
                    DDI = cliente.Telefone.DDI,
                    Numero = cliente.Telefone.Numero
                },

                Endereco = new EnderecoDTO
                {
                    Bairro = cliente.Endereco.Bairro,
                    Cep = cliente.Endereco.Cep,
                    Cidade = cliente.Endereco.Cidade,
                    Complemento = cliente.Endereco.Complemento,
                    Logradouro = cliente.Endereco.Logradouro,
                    Numero = cliente.Endereco.Numero,
                    Uf = cliente.Endereco.Uf
                }
            };
        }

        public static List<ClienteResponseDTO> ToListDTO(this ICollection<Cliente> clientes)
        {
            return clientes.Select(c => c.ToDto()).ToList();
        }
    }
}

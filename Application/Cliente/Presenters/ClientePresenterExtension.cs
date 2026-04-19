using Application.Cliente.DTOs.Responses;
using Application.Cliente.DTOs.Shared;
using Domain.Agregates.Cliente;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Cliente.Presenters
{
    public static class ClientePresenterExtension
    {
        public static ClienteResponseDTO ToDto(this ClienteEntity cliente)
        {
            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                CpfCnpj = cliente.CpfCnpj.ToString(),
                Emails = cliente.Emails != null ? cliente.Emails.Select(e => e.EnderecoEmail).ToList() : new List<string>(),

                Telefones = cliente.Telefones != null ? cliente.Telefones.Select(t => new TelefoneDTO
                {
                    DDD = t.DDD,
                    DDI = t.DDI,
                    Numero = t.Numero,
                    Tipo = t.Tipo
                }).ToList() : new List<TelefoneDTO>(),

                Enderecos = cliente.Enderecos != null ? cliente.Enderecos.Select(e => new EnderecoDTO
                {
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Cep = e.Cep
                }).ToList() : new List<EnderecoDTO>()
            };
        }

        public static List<ClienteResponseDTO> ToListDTO(this IEnumerable<ClienteEntity> clientes)
        {
            return clientes.Select(c => c.ToDto()).ToList();
        }
    }
}

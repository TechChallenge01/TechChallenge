using Application.Cliente.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Cliente.DTOs.Requests
{
    public record ClienteRequestDTO
    {
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public ICollection<string> Emails { get; set; }
        public ICollection<TelefoneDTO> Telefones { get; set; }
        public ICollection<EnderecoDTO> Enderecos { get; set; }
    }
}

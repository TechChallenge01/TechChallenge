using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Cliente.DTOs.Shared
{
    public record TelefoneDTO
    {
        public string DDD { get;  set; }
        public string DDI { get;  set; }
        public string Numero { get;  set; }
        public string Tipo { get;  set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Email
    {
        public Email(string email, Guid clienteId)
        {
            EnderecoEmail = email;
            ClienteId = clienteId;
        }

        public virtual Cliente Cliente { get; private set; }
        public Guid ClienteId { get; private set; }
        public string EnderecoEmail { get; private set; }
    }
}

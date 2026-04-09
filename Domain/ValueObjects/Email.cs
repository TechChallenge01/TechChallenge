using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ValueObjects
{
    public class Email
    {
        public Email(string email, Guid clienteId)
        {
            EnderecoEmail = email;
        }

        public string EnderecoEmail { get; private set; }
    }
}

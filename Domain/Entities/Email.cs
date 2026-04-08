using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Email
    {
        public Email(string email)
        {
            Id = Guid.NewGuid();
            EnderecoEmail = email;
        }
        public Guid Id { get; private set; }
        public string EnderecoEmail { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Endereco
    {
        public Endereco(string logradouro, string numero, string complemento, string bairro , string cidade, string estado, string cep)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Uf = estado;
            Cep = cep;
        }

        protected Endereco() { }
        public Guid ClienteId { get; private set; }
        public virtual Cliente Cliente { get; private set; }
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }
        public string Bairro { get; private set; }
        public string Cep { get; private set; }
        public string Cidade { get; private set; }
        public string Uf { get; private set; }
    }
}

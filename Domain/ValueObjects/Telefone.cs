using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ValueObjects
{
    public class Telefone
    {
        public Telefone(string ddd, string ddi, string numero, string tipo, Guid clienteId)
        {
            DDD = ddd;
            DDI = ddi;
            Numero = numero;
            Tipo = tipo; // Celular, Fixo, etc.
        }

        protected Telefone() { }
        public string DDD { get; private set; }
        public string DDI { get; private set; }
        public string Numero { get; private set; }
        public string Tipo { get; private set; }
    }
}

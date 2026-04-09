using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.ValueObjects
{
    public class Telefone
    {
        public string DDD { get; private set; }
        public string DDI { get; private set; }
        public string Numero { get; private set; }
        public TipoTelefone Tipo { get; private set; }

        public Telefone(string ddd, string ddi, string numero, TipoTelefone tipo)
        {
            if(string.IsNullOrWhiteSpace(ddd))
                throw new ArgumentException("O DDD não pode ser nulo ou vazio.");
            if(string.IsNullOrWhiteSpace(ddi))
                throw new ArgumentException("O DDI não pode ser nulo ou vazio.");
            if(string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("O número não pode ser nulo ou vazio.");

            DDD = ddd.Trim();
            DDI = ddi.Trim();
            Numero = numero.Trim();
            Tipo = tipo; 
        }

        protected Telefone() { }

        public override bool Equals(object obj)
        {
            if (obj is Telefone outro) 
            {
                return DDI == outro.DDI && 
                       DDD == outro.DDD && 
                       Numero == outro.Numero;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DDI, DDD, Numero);
        }

    }
}

namespace Domain.ValueObjects
{
    public class Telefone
    {

        public Telefone(string ddd, string ddi, string numero)
        {
            numero = numero.Replace("-", "").Replace("(", "").Replace(")", "");

            if (string.IsNullOrWhiteSpace(ddd))
                throw new ArgumentException("O DDD não pode ser nulo ou vazio.");
            if (string.IsNullOrWhiteSpace(ddi))
                throw new ArgumentException("O DDI não pode ser nulo ou vazio.");
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("O número não pode ser nulo ou vazio.");
            validarTelefone(numero);

            DDD = ddd.Trim();
            DDI = ddi.Trim();
            Numero = numero.Trim();
        }

        protected Telefone() { }

        public string DDD { get; private set; }
        public string DDI { get; private set; }
        public string Numero { get; private set; }

        private void validarTelefone(string numero)
        {
            if (numero.Length < 8 || numero.Length > 9)
                throw new ArgumentException("Numero de telefone inválido");
        }

    }
}

namespace Domain.ValueObjects
{
    public class Placa
    {
        protected Placa() { }

        public Placa(string valor) 
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("Placa não pode ser vazia");

            valor = valor.Replace("-", "").Replace(" ", "").ToUpper();

            if (!EhValida(valor))
                throw new ArgumentException("Placa inválida");

            Valor = valor;
        }   

        public string Valor { get; private set; }


        private static bool EhValida(string placa)
        {
            return EhFormatoAntigo(placa) || EhFormatoMercosul(placa);
        }
        private static bool EhFormatoAntigo(string placa)
        {
            return System.Text.RegularExpressions.Regex
                .IsMatch(placa, @"^[A-Z]{3}[0-9]{4}$");
        }

        private static bool EhFormatoMercosul(string placa)
        {
            return System.Text.RegularExpressions.Regex
                .IsMatch(placa, @"^[A-Z]{3}[0-9][A-Z][0-9]{2}$");
        }

        public override string ToString()
        {
            return Valor.ToString();
        }
    }
}

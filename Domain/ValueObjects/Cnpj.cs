namespace Domain.ValueObjects
{
    public class Cnpj
    {
        protected Cnpj() { }

        public Cnpj(string cnpj) 
        { 
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "")
            ValidarCnpj(cnpj);

            Valor = cnpj;
        }    


        public string Valor { get; private set; }
        private void ValidarCnpj(string cnpj)
        {
            if (cnpj.Length != 14)
                throw new ArgumentException("Cnpj deve conter 14 dígitos.", nameof(cnpj));

            if(string.IsNullOrEmpty(cnpj))
                throw new ArgumentException("O CNPJ não pode ser vazio.", nameof(cnpj));

            if (cnpj.Distinct().Count() == 1)
                throw new ArgumentException("CNPJ inválido.", nameof(cnpj));

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += (tempCnpj[i] - '0') * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCnpj += digito;

            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += (tempCnpj[i] - '0') * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            if (!cnpj.EndsWith(digito))
                throw new ArgumentException("CNPJ inválido.", nameof(cnpj));
        }
    }
}

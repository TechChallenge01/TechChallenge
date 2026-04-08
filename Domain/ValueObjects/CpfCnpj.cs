namespace Domain.ValueObjects
{
    public class CpfCnpj
    {
        public CpfCnpj(string cpfCnpj)
        {
            ValidarCpfCnpj(cpfCnpj);

            Valor = cpfCnpj;
        }

        protected CpfCnpj() { }


        public string Valor { get; private set; }

        private void ValidarCpfCnpj(string cpfCnpj)
        {
            if (string.IsNullOrEmpty(cpfCnpj))
                throw new ArgumentException("O CPF/CNPJ não pode ser vazio.");

            if (cpfCnpj.Length == 11)
                ValidarCpf(cpfCnpj);

            if (cpfCnpj.Length == 14)
                ValidarCnpj(cpfCnpj);
        }

        private void ValidarCpf(string cpf)
        {
            if (cpf.Distinct().Count() == 1)
                throw new ArgumentException("CPF inválido.");

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += (tempCpf[i] - '0') * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += (tempCpf[i] - '0') * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            if (!cpf.EndsWith(digito))
                throw new ArgumentException("CPF inválido.");
        }
        private void ValidarCnpj(string cnpj)
        {
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

namespace Domain
{
    public class Cliente
    {
        public Cliente(string nome, string cpfCnpj)
        {
            cpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());
            ValidarNome(nome);

            Id = Guid.NewGuid();
            Nome = nome;
            CpfCnpj = new CpfCnpj(cpfCnpj);
        }

        protected Cliente() { }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public CpfCnpj CpfCnpj { get; private set; }

        
        private void ValidarNome(string nome)
        {
            if(string.IsNullOrEmpty(nome))
            {
                throw new ArgumentException("O nome do cliente não pode ser vazio.");
            }
        }
    }
}
using Domain.ValueObjects;
using System.Linq;

namespace Domain.Entities
{
    public class Cliente
    {
        private readonly List<Email> _emails;
        private readonly List<Telefone> _telefones;
        private readonly List<Endereco> _enderecos;
        public Cliente(string nome, string cpfCnpj)
        {
            cpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());
            ValidarNome(nome);

            Id = Guid.NewGuid();
            Nome = nome;
            CpfCnpj = new CpfCnpj(cpfCnpj);

            _emails = new List<Email>();
            _telefones = new List<Telefone>();
            _enderecos = new List<Endereco>();
        }

        protected Cliente() 
        {
            _emails = new List<Email>();
            _telefones = new List<Telefone>();
            _enderecos = new List<Endereco>();
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public CpfCnpj CpfCnpj { get; private set; }


        public IReadOnlyCollection<Email> Emails => _emails.AsReadOnly();
        public IReadOnlyCollection<Telefone> Telefones => _telefones.AsReadOnly();
        public IReadOnlyCollection<Endereco> Enderecos => _enderecos.AsReadOnly();

        public void AdicionarEndereco(Endereco endereco)
        {
            if(endereco == null) throw new ArgumentNullException(nameof(endereco)) ;
            _enderecos.Add(endereco);
        }

        public void AdicionarEmail(Email email)
        {
            if(email == null) throw new ArgumentNullException(nameof(email)) ;
            _emails.Add(email);
        }

        public void AdicionarTelefone(Telefone telefone)
        {
            if(telefone == null) throw new ArgumentNullException(nameof(telefone)) ;
            _telefones.Add(telefone);
        }

        private void ValidarNome(string nome)
        {
            if(string.IsNullOrEmpty(nome))
            {
                throw new ArgumentException("O nome do cliente não pode ser vazio.");
            }
        }
    }
}
using Domain.Aggregates.Cliente;
using Domain.Aggregates.Cliente.ValueObjects;
using Domain.Base;

namespace Domain.Agregates.Cliente
{
    public class ClienteEntity : BaseEntity
    {
        public ClienteEntity(string nome, string cpfCnpj, Guid idUsuarioCriacao)
        {
            cpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());
            ValidarNome(nome);

            Id = Guid.NewGuid();
            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = nome;
            CpfCnpj = new CpfCnpj(cpfCnpj);
        }

        protected ClienteEntity()
        { }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public CpfCnpj CpfCnpj { get; private set; }

        public ICollection<Email> Emails { get; private set; } = new List<Email>();
        public ICollection<Telefone> Telefones { get; private set; } = new List<Telefone>();
        public ICollection<Endereco> Enderecos { get; private set; } = new List<Endereco>();

        public void AdicionarEndereco(List<Endereco> enderecos)
        {
            if(enderecos == null) 
                throw new ArgumentNullException(nameof(enderecos)) ;

            Enderecos = enderecos;
        }

        public void AdicionarEmail(List<Email> emails)
        {
            if(emails == null) 
                throw new ArgumentNullException(nameof(emails)) ;

            Emails = emails;
        }

        public void AdicionarTelefone(List<Telefone> telefones)
        {
            if(telefones == null) 
                throw new ArgumentNullException(nameof(telefones)) ;

            Telefones = telefones;
        }

        public void AlterarNome(string nome)
        {
            ValidarNome(nome);
            Nome = nome;
        }

        public void AlterarCpfCnpj(string cpfCnpj)
        {
            cpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());
            CpfCnpj = new CpfCnpj(cpfCnpj);
        }

        private void ValidarNome(string nome)
        {
            if(string.IsNullOrEmpty(nome))
                throw new ArgumentException("O nome do cliente não pode ser vazio.");
        }
    }
}
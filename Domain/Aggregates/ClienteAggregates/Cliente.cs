using Domain.BaseEntity;
using Domain.ValueObjects;


namespace Domain.Aggregates.ClienteAggregates
{
    public class Cliente : Base
    {
        private Cliente(string nome, Guid idUsuarioCriacao, ICollection<Email> Emails, ICollection<Telefone> Telefones, ICollection<Endereco>? Enderecos)
        {
            ValidarNome(nome);
            Id = Guid.NewGuid();
            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = nome;

            Emails = Emails;
            Telefones = Telefones;
            Enderecos = Enderecos;
        }

        public Cliente(string nome, Cpf cpf, Guid idUsuarioCriacao, ICollection<Email> Emails, ICollection<Telefone> Telefones, ICollection<Endereco>? Enderecos) : this(nome, idUsuarioCriacao, Emails, Telefones, Enderecos)
        {
            Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        }

        public Cliente(string nome, Cnpj cnpj, Guid idUsuarioCriacao, ICollection<Email> Emails, ICollection<Telefone> Telefones, ICollection<Endereco>? Enderecos) : this(nome, idUsuarioCriacao, Emails, Telefones, Enderecos)
        {
            Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        }

        protected Cliente()
        { }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Cpf? Cpf { get; private set; }
        public Cnpj? Cnpj { get; private set; }
        public ICollection<Email> Emails { get; private set; } = new List<Email>();
        public ICollection<Telefone> Telefones { get; private set; } = new List<Telefone>();
        public ICollection<Endereco> Enderecos { get; private set; } = new List<Endereco>();

        public void AlterarEnderecos(List<Endereco> enderecos)
        {
            if(enderecos == null) 
                throw new ArgumentNullException(nameof(enderecos)) ;

            Enderecos = enderecos
                        .DistinctBy(e => new { e.Logradouro, e.Numero, e.Bairro, e.Cidade, e.Uf, e.Cep })
                        .ToList();
        }

        public void AlterarEmails(List<Email> emails)
        {
            if(emails == null) 
                throw new ArgumentNullException(nameof(emails)) ;

            Emails = emails
                     .DistinctBy(e => e.EnderecoEmail).ToList();
        }

        public void AlterarTelefones(List<Telefone> telefones)
        {
            if(telefones == null) 
                throw new ArgumentNullException(nameof(telefones)) ;

            Telefones = telefones
                        .DistinctBy(t => new {t.DDD, t.Numero, t.DDI, t.Tipo})
                        .ToList();
        }

        public void AlterarNome(string nome)
        {
            ValidarNome(nome);
            Nome = nome;
        }

        private void ValidarNome(string nome)
        {
            if(string.IsNullOrEmpty(nome))
                throw new ArgumentException("O nome do cliente não pode ser vazio.");
        }
    }
}
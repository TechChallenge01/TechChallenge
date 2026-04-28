using Domain.BaseEntity;
using Domain.Entities;
using Domain.ValueObjects;


namespace Domain.Aggregates.ClienteAggregates
{
    public class Cliente : Base
    {
        private Cliente(string nome, Guid idUsuarioCriacao) : base(idUsuarioCriacao, DateTime.UtcNow, null, null)
        {
            ValidarNome(nome);
            Id = Guid.NewGuid();
            Nome = nome;

            Emails = Emails;
            Telefones = Telefones;
            Enderecos = Enderecos;
        }

        public Cliente(string nome, Cpf cpf, Guid idUsuarioCriacao) : this(nome, idUsuarioCriacao)
        {
            Cpf = cpf ?? throw new ArgumentException("cpf não pode ser nulo!");
        }

        public Cliente(string nome, Cnpj cnpj, Guid idUsuarioCriacao) : this(nome, idUsuarioCriacao)
        {
            Cnpj = cnpj ?? throw new ArgumentException("cnpj não pode ser nulo!");
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
        public ICollection<Veiculo>? Veiculos { get; private set; }

        public void AlterarEnderecos(List<Endereco> enderecos)
        {
            if(enderecos == null) 
                throw new ArgumentException("enderecos não pode ser nulo!") ;

            Enderecos = enderecos
                        .DistinctBy(e => new { e.Logradouro, e.Numero, e.Bairro, e.Cidade, e.Uf, e.Cep })
                        .ToList();
        }

        public void AlterarEmails(List<Email> emails)
        {
            if(emails == null) 
                throw new ArgumentException("emails não pode ser nulo!") ;

            Emails = emails
                     .DistinctBy(e => e.EnderecoEmail).ToList();
        }

        public void AlterarTelefones(List<Telefone> telefones)
        {
            if(telefones == null) 
                throw new ArgumentException("telefones não pode ser nulo!") ;

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
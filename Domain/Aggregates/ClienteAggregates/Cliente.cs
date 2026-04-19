using Domain.ValueObjects;

namespace Domain.Aggregates.ClienteAggregates
{
    public class Cliente : Base.Base
    {
        public Cliente(string nome, string? cpf, string? cnpj, Guid idUsuarioCriacao)
        {
            ValidarNome(nome);

            Id = Guid.NewGuid();
            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = nome;

            if(cpf is not null)
                Cpf = new Cpf(cpf);
            else
                Cnpj = new Cnpj(cnpj);
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

        public void AdicionarEndereco(List<Endereco> enderecos)
        {
            if(enderecos == null) 
                throw new ArgumentNullException(nameof(enderecos)) ;

            Enderecos = enderecos
                        .DistinctBy(e => new { e.Logradouro, e.Numero, e.Bairro, e.Cidade, e.Uf, e.Cep })
                        .ToList();
        }

        public void AdicionarEmail(List<Email> emails)
        {
            if(emails == null) 
                throw new ArgumentNullException(nameof(emails)) ;

            Emails = emails
                     .DistinctBy(e => e.EnderecoEmail).ToList();
        }

        public void AdicionarTelefone(List<Telefone> telefones)
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
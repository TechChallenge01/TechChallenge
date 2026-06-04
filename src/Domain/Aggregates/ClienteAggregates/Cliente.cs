using Domain.Aggregates.OrdemServicoAggregates;
using Domain.BaseEntity;
using Domain.Entities;
using Domain.ValueObjects;


namespace Domain.Aggregates.ClienteAggregates
{
    public class Cliente : Base
    {
        private Cliente(string nome, Guid idUsuarioCriacao, Endereco endereco, Telefone telefone, Email email) : base(idUsuarioCriacao, DateTime.UtcNow, null, null)
        {
            ValidarNome(nome);
            Id = Guid.NewGuid();
            Nome = nome;
            Endereco = endereco;
            Telefone = telefone;
            Email = email;
        }

        public Cliente(string nome, Cpf cpf, Guid idUsuarioCriacao, Endereco endereco, Telefone telefone, Email email) : this(nome, idUsuarioCriacao, endereco, telefone, email)
        {
            Cpf = cpf ?? throw new ArgumentException("cpf não pode ser nulo!");
        }

        public Cliente(string nome, Cnpj cnpj, Guid idUsuarioCriacao, Endereco endereco, Telefone telefone, Email email) : this(nome, idUsuarioCriacao, endereco, telefone, email)
        {
            Cnpj = cnpj ?? throw new ArgumentException("cnpj não pode ser nulo!");
        }

        public Cliente(Guid id, string nome, Cpf? cpf, Cnpj? cnpj, Email email, Telefone telefone, Endereco endereco, ICollection<Veiculo>? veiculos)
        {
            ValidarNome(nome);

            Id = id;
            Nome = nome;
            Cpf = cpf;
            Cnpj = cnpj;
            Email = email;
            Telefone = telefone;
            Endereco = endereco;
            Veiculos = veiculos;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Cpf? Cpf { get; private set; }
        public Cnpj? Cnpj { get; private set; }
        public Email Email { get; private set; } 
        public Telefone Telefone { get; private set; }
        public Endereco Endereco { get; private set; }
        public ICollection<Veiculo>? Veiculos { get; private set; } = new List<Veiculo>();
        public ICollection<OrdemServico> OrdemServicos = new List<OrdemServico>();

        public void AlterarEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public void AlterarEmail(Email email)
        {
            Email = email;
        }

        public void AlterarTelefone(Telefone telefone)
        {
            Telefone = telefone;
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
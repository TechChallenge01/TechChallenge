using Domain.Entities;

namespace Infra.DataModel
{
    public class ClienteDbModel
    {
        public ClienteDbModel(Guid id, string nome, string? cpf, string? cnpj, string email, string dDD, string dDI, string numeroTelefone, string logradouro, string numero, string? complemento, string bairro, string cep, string cidade, string uf, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Cnpj = cnpj;
            Email = email;
            DDD = dDD;
            DDI = dDI;
            NumeroTelefone = numeroTelefone;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cep = cep;
            Cidade = cidade;
            Uf = uf;
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
            Ativo = true;
        }

        protected ClienteDbModel() { }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Cnpj { get; set; }
        public string Email { get; set; }
        public string DDD { get; private set; }
        public string DDI { get; private set; }
        public string NumeroTelefone { get; private set; }
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string? Complemento { get; private set; }
        public string Bairro { get; private set; }
        public string Cep { get; private set; }
        public string Cidade { get; private set; }
        public string Uf { get; private set; }
        public ICollection<Veiculo>? Veiculos { get; set; } = new List<Veiculo>();
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}

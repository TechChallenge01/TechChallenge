using Domain.Aggregates.Cliente.ValueObjects;
using Domain.Base;
using System.Linq;

namespace Domain.Agregates.Cliente
{
    public class ClienteEntity : BaseEntity
    {
        public ClienteEntity(string nome, string cpfCnpj, Guid idUsuarioCriacao)
        {
            cpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());
            ValidarNome(nome);

            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = nome;
            CpfCnpj = new CpfCnpj(cpfCnpj);

            _emails = new List<Email>();
            _telefones = new List<Telefone>();
            _enderecos = new List<Endereco>();
            _veiculos = new List<Veiculo>();
        }

        protected ClienteEntity()
        {
            _emails = new List<Email>();
            _telefones = new List<Telefone>();
            _enderecos = new List<Endereco>();
            _veiculos = new List<Veiculo>();
        }

        public string Nome { get; private set; }
        public CpfCnpj CpfCnpj { get; private set; }

        private readonly List<Email> _emails;
        private readonly List<Telefone> _telefones;
        private readonly List<Endereco> _enderecos;
        private readonly List<Veiculo> _veiculos;

        public IReadOnlyCollection<Email> Emails => _emails.AsReadOnly();
        public IReadOnlyCollection<Telefone> Telefones => _telefones.AsReadOnly();
        public IReadOnlyCollection<Endereco> Enderecos => _enderecos.AsReadOnly();
        public IReadOnlyCollection<Veiculo> Veiculos => _veiculos.AsReadOnly();
        public void AdicionarEndereco(Endereco endereco)
        {
            if(endereco == null) throw new ArgumentNullException(nameof(endereco)) ;

            bool enderecoExistente = _enderecos.Any(enderecoTemporario => 
                enderecoTemporario.Logradouro.Equals(endereco.Logradouro, StringComparison.OrdinalIgnoreCase) &&
                enderecoTemporario.Numero.Equals(endereco.Numero, StringComparison.OrdinalIgnoreCase) &&
                enderecoTemporario.Cep.Equals(endereco.Cep, StringComparison.OrdinalIgnoreCase));

            if (enderecoExistente) { throw new ArgumentException("Este endereço já está cadastrado."); }

            _enderecos.Add(endereco);
        }

        public void AdicionarEmail(Email email)
        {
            if(email == null) throw new ArgumentNullException(nameof(email)) ;

            bool emailExistente = _emails.Any(emailTemporario => emailTemporario.EnderecoEmail.Equals(email.EnderecoEmail, StringComparison.OrdinalIgnoreCase));

            if(emailExistente) { throw new ArgumentException("Este e-mail já está cadastrado."); }

            _emails.Add(email);
        }

        public void AdicionarTelefone(Telefone telefone)
        {
            if(telefone == null) throw new ArgumentNullException(nameof(telefone)) ;

            bool telefoneExistente = _telefones.Any(telefoneTemporario => 
                telefoneTemporario.DDD == telefone.DDD &&
                telefoneTemporario.DDI == telefone.DDI &&
                telefoneTemporario.Numero == telefone.Numero);

            if (telefoneExistente) { throw new ArgumentException("Este telefone já está cadastrado."); }

            _telefones.Add(telefone);
        }

        public void AdicionarVeiculo(Veiculo veiculo)
        {
            if(veiculo == null) throw new ArgumentNullException(nameof(veiculo)) ;

            bool veiculoExistente = _veiculos.Any(veiculoTemporario => veiculoTemporario.Placa.Equals(veiculo.Placa, StringComparison.OrdinalIgnoreCase));

            if (veiculoExistente) { throw new ArgumentException("Este veículo já está cadastrado."); }

            _veiculos.Add(veiculo);
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
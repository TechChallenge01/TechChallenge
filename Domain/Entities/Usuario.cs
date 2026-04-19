using Domain.Enums;
using Domain.Base;
using Domain.Aggregates.Cliente.ValueObjects;

namespace Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public Usuario(string name, string email, string senha, EPerfilUsuario perfil, Guid idUsuarioCriacao)
        {
            ValidarNome(name);
            ValidarEmail(email);
            ValidarSenha(senha);

            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = name;
            Email = new Email(email);
            SenhaHash = senha;
            Perfil = perfil;
        }

        public Usuario() { }

        public string Nome { get; private set; }
        public Email Email { get; private set; }
        public string SenhaHash { get; private set; }
        public EPerfilUsuario Perfil { get; private set; }
        

        private void ValidarNome(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
        }

        private void ValidarEmail(string email) 
        {
            Email = new Email(email);
        }

        private void ValidarSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6 )
                throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.");
        }
    }
}

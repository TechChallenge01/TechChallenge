using Domain.BaseEntity;
using Domain.Enums;

namespace Domain.Entities
{
    public class Usuario : Base
    {
        public Usuario(string name, string email, string senha, EPerfilUsuario perfil, Guid idUsuarioCriacao) 
        {

            ValidarNome(name);
            ValidarEmail(email);
            ValidarSenha(senha);

            Id = Guid.NewGuid();
            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = name;
            Email = email;
            SenhaHash = senha;
            Perfil = perfil.ToString();
        }

        public Usuario(Guid id, string nome, string email, string senhaHash, string perfil)
        {
            Id = id;
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            Perfil = perfil;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public string Perfil { get; private set; }
        

        private void ValidarNome(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do usuário não pode ser nulo ou vazio.");
        }

        private void ValidarEmail(string email)
        {
            try
            {
                new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                throw new ArgumentException("O email fornecido é inválido.");
            }
        }

        private void ValidarSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6 )
                throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.");
        }
    }
}

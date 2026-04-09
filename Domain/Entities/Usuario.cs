using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Domain.ValueObjects;
using Domain.Enums;
using System.Runtime.CompilerServices;

namespace Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Email Email { get; private set; }
        public string SenhaHash { get; private set; }
        public PerfilUsuario Perfil { get; private set; }

        public Usuario(string name, string email, string senha, PerfilUsuario perfil)
        {
            ValidarNome(name);
            ValidarEmail(email);
            ValidarSenha(senha);

            Id = Guid.NewGuid();
            Nome = name;
            Email = new Email(email);
            SenhaHash = senha;
            Perfil = perfil;
        }

        public Usuario() { }

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
            if (string.IsNullOrWhiteSpace(senha) || senha.Length > 6 )
                throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.");
        }
    }
}

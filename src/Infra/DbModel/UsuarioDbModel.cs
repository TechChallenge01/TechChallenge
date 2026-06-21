using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.DbModel
{
    public class UsuarioDbModel
    {
        public UsuarioDbModel() { }

        public UsuarioDbModel(Guid id, string nome, string email, string senhaHash, string perfil, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao, bool ativo)
        {
            Id = id;
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            Perfil = perfil;
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
            Ativo = ativo;
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public string Perfil { get; set; }
        public Guid IdUsuarioCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid? IdUsuarioAtualizacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }

    }
}

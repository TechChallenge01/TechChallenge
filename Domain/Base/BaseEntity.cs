using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Base
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        { }

        protected BaseEntity(Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao)
        {
            IdUsuarioCriacao = idUsuarioCriacao;
            DataCriacao = dataCriacao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }

        public Guid IdUsuarioCriacao { get; protected set; }
        public DateTime DataCriacao { get; protected set; }

        public Guid? IdUsuarioAtualizacao { get; protected set; }
        public DateTime? DataAtualizacao { get; protected set; }

        public bool Ativo { get; protected set; } = true;

        public void Inativar() => Ativo = false;

    }
}

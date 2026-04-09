using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        public Guid IdUsuarioCriacao { get; protected set; }
        public DateTime DataCriacao { get; protected set; }

        public Guid? IdUsuarioAtualizacao { get; protected set; }
        public DateTime? DataAtualizacao { get; protected set; }

        public bool Ativo { get; protected set; }

        public Entity()
        {
            Id = Guid.NewGuid();
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
        }

        public void Inativar() => Ativo = false;

    }
}

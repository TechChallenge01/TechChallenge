namespace Domain.BaseEntity
{
    public abstract class Base
    {
        public Base()
        { }

        protected Base(Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao)
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

        public void Inativar(Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {
            Ativo = false;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        } 
        public void RastrearAlteracao(Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }
    }
}

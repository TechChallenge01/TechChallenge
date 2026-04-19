namespace Domain.Entities
{
    public class MarcaVeiculo : Base.Base
    {
        protected MarcaVeiculo() { }

        public MarcaVeiculo(string nome, Guid idUsuarioCriacao)
        {
            ValidarMarca(nome);

            Id = Guid.NewGuid();
            IdUsuarioCriacao = idUsuarioCriacao;
            Nome = nome.Trim();
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }

        private void ValidarMarca(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da marca é obrigatório.");
        }
    }
}

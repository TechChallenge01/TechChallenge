using Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ServicoEntity : BaseEntity
    {
        public ServicoEntity() { }

        public ServicoEntity(string nome, string descricao, decimal precoVenda, Guid idUsuarioCriacao, DateTime dataCriacao) : base(idUsuarioCriacao, dataCriacao, null, null)
        {

            ValidarDescricao(descricao);
            ValidarNome(nome);
            ValidarPrecoVenda(precoVenda);

            Id = Guid.NewGuid();

            Nome = nome;
            Descricao = descricao;
            PrecoVenda = precoVenda;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public decimal PrecoVenda { get; private set; }
        public TimeSpan? TempoMedioExecucao { get; private set; }


        private void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do serviço é obrigatório.");
        }
        private void ValidarPrecoVenda(decimal precoVenda)
        {
            if (precoVenda <= 0)
                throw new ArgumentException("O preço de venda deve ser maior que zero.");
        }
        private void ValidarDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição do serviço é obrigatória.");
        }


        public void AlterarPrecoVenda(decimal novoPrecoVenda, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {

            ValidarPrecoVenda(novoPrecoVenda);

            PrecoVenda = novoPrecoVenda;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }
        public void AlterarDescricao(string novaDescricao, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {
            ValidarDescricao(novaDescricao);

            Descricao = novaDescricao;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }
        public void AlterarNome(string novoNome, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
        {
            ValidarNome(novoNome);

            Nome = novoNome;
            IdUsuarioAtualizacao = idUsuarioAtualizacao;
            DataAtualizacao = dataAtualizacao;
        }
        public void AtualizarTempoMedio(List<TimeSpan> tempos)
        {
            if (!tempos.Any())
                return;

            TempoMedioExecucao = TimeSpan.FromTicks(
                (long)tempos.Average(t => t.Ticks)
            );
        }
    }
}

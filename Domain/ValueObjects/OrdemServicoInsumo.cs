using Domain.BaseEntity;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ValueObjects
{
    public class OrdemServicoInsumo : Base
    {
        public OrdemServicoInsumo(Guid servicoId, int quantidade, decimal custoUnitario, Guid IdUsuarioCriacao)
        {
            ValidarQuantidade(quantidade);
            ValidarCustoUnitario(custoUnitario);

            ServicoId = servicoId;
            Quantidade = quantidade;
            CustoUnitario = custoUnitario;
            IdUsuarioCriacao = IdUsuarioCriacao;
            DataCriacao = DateTime.UtcNow;
        }

        protected OrdemServicoInsumo() { }


        public Guid ServicoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal CustoUnitario { get; private set; }
        public decimal ValorTotal => Quantidade * CustoUnitario;
        public virtual Insumo insumo { get; private set; }
        public string NomeInsumo => insumo?.Nome;
        public string DescricaoInsumo => insumo?.Descricao;

        private void ValidarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero.");
        }

        private void ValidarCustoUnitario(decimal custoUnitario)
        {
            if (custoUnitario <= 0)
                throw new ArgumentException("Custo unitário deve ser positivo.");
        }
    }
}

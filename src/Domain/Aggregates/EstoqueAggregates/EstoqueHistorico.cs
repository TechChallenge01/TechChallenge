using Domain.BaseEntity;
using Domain.Enums;

namespace Domain.Aggregates.EstoqueAggregates;

public class EstoqueHistorico : Base
{
    public EstoqueHistorico( int quantidade, string observacao, ETipoMovimentacao tipoMovimentacao, Guid UsuarioCriacaoId, DateTime dataCriacao, Guid estoqueId) : base(UsuarioCriacaoId, dataCriacao, null, null)
    {
        ValidarQuantidade(quantidade);

        Id = Guid.NewGuid();
        Quantidade = quantidade;
        Observacao = observacao;
        TipoMovimentacao = tipoMovimentacao.ToString();
        EstoqueId = estoqueId;
    }

    public Guid Id { get; private set; }
    public int Quantidade { get; private set; } 
    public string Observacao { get; private set; } = string.Empty;
    public string TipoMovimentacao { get; private set; }
    public Guid EstoqueId { get; private set; }
    public virtual Estoque Estoque {  get; private set; }

    private void ValidarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");
    }
}

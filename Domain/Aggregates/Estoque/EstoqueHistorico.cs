using Domain.BaseEntity;
using Domain.Enums;

namespace Domain.Aggregates.Estoque;

public class EstoqueHistorico : Base
{
    public Guid EstoqueId { get; private set; }
    public int Quantidade { get; private set; } 
    public string Observacao { get; private set; } = string.Empty;
    //a ser imprementado codigo da OS
    public ETipoMovimentacao TipoMovimentacao { get; private set; }

    public EstoqueHistorico(Guid estoqueId, int quantidade, string observacao, ETipoMovimentacao tipoMovimentacao, Guid UsuarioCriacaoId, DateTime dataCriacao) : base(UsuarioCriacaoId, dataCriacao, null, null)
    {
        ValidarEstoqueId(estoqueId);
        ValidarQuantidade(quantidade);

        EstoqueId = estoqueId;
        Quantidade = quantidade;
        Observacao = observacao;
        TipoMovimentacao = tipoMovimentacao;
    }

    protected EstoqueHistorico() { }

    private void ValidarEstoqueId(Guid estoqueId)
    {
        if (estoqueId == Guid.Empty)
            throw new ArgumentException("O Id do estoque não pode ser vazio.");
    }

    private void ValidarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");
    }
}

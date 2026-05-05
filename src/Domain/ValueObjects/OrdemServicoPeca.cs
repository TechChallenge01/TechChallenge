using Domain.Aggregates.OrdemServicoAggregates;
using Domain.Entities;

namespace Domain.ValueObjects;

public class OrdemServicoPeca
{
    public Guid OrdemServicoId { get; private set; }
    public Guid PecaId { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
    public virtual Peca Peca { get; private set; }
    public virtual OrdemServico OrdemServico { get; private set; }

    public OrdemServicoPeca(Guid osId, Guid pecaId, int quantidade, decimal valorUnitario)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero.");
        if (valorUnitario <= 0) throw new ArgumentException("Valor unitário deve ser positivo.");

        OrdemServicoId = osId;
        PecaId = pecaId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }
    protected OrdemServicoPeca() { }
}

using Domain.BaseEntity;

namespace Domain.Aggregates.OrdemServico;

public class OsPeca : Base
{
    public Guid OsId { get; private set; }
    public Guid PecaId { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
    public OsPeca(Guid osId, Guid pecaId, int quantidade, decimal valorUnitario, Guid idUsuarioCriacao)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero.");
        if (valorUnitario <= 0) throw new ArgumentException("Valor unitário deve ser positivo.");

        OsId = osId;
        PecaId = pecaId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        IdUsuarioCriacao = idUsuarioCriacao;
        DataCriacao = DateTime.UtcNow;
    }
    protected OsPeca() { }
}

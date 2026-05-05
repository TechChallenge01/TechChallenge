using Domain.BaseEntity;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Aggregates.EstoqueAggregates;
public class Estoque : Base
{
    public Estoque(Guid? insumoId, Guid? pecaId, int quantidadeDisponivel, Guid UsuarioCriacaoId, DateTime dataCriacao) : base(UsuarioCriacaoId, dataCriacao, null, null)
    {
        if (insumoId is null && pecaId is null)
            throw new ArgumentException("É obrigatório o uso de uma peca ou um Insumo");

        if (insumoId is not null && pecaId is not null)
            throw new ArgumentException("Não é permitido ter uma peca e um Insumo em um estoque!");

        if(pecaId is not null)
            ValidarPecaId((Guid)pecaId);

        if(Insumo is not null)
            ValidarInsumoId((Guid)insumoId);

        ValidarQuantidadeDisponivel(quantidadeDisponivel);

        Id = Guid.NewGuid();
        PecaId = pecaId;
        InsumoId = insumoId;
        QuantidadeDisponivel = quantidadeDisponivel;
        QuantidadeReservada = 0;
    }

    protected Estoque() { }

    public Guid Id { get; private set; }
    public Guid? PecaId { get; private set; }
    public Guid? InsumoId {  get; private set; }
    public int QuantidadeDisponivel { get; private set; }
    public int QuantidadeReservada { get; private set; }
    public ICollection<EstoqueHistorico> Historicos { get; private set; } = new List<EstoqueHistorico>();
    public virtual Peca Peca { get; private set; }
    public virtual Insumo Insumo { get; private set; }
    public int QuantidadeTotal => QuantidadeDisponivel + QuantidadeReservada;

    private void ValidarPecaId(Guid pecaId)
    {
        if (pecaId == Guid.Empty)
            throw new ArgumentException("O ID da peça não pode ser vazio.");
    }
    private void ValidarInsumoId(Guid insumoId)
    {
        if (insumoId == Guid.Empty)
            throw new ArgumentException("O ID do Insumo não pode ser vazio.");
    }

    private void ValidarQuantidadeDisponivel(int quantidadeDisponivel) 
    {
        if(quantidadeDisponivel < 0)
            throw new ArgumentException("A quantidade disponível não pode ser negativa.");
    }

    private void ValidarQuantidadeReservada(int quantidadeReservada) 
    {
        if(quantidadeReservada < 0)
            throw new ArgumentException("A quantidade reservada não pode ser negativa.");
    }

    public void AdicionarEstoque(int quantidade, Guid usuarioCriacaoId)
    {
        ValidarQuantidadeDisponivel(quantidade);

        QuantidadeDisponivel += quantidade;
        AdicionarMovimentacao(quantidade, "Adição de estoque", ETipoMovimentacao.Entrada, usuarioCriacaoId, DateTime.UtcNow);
    }

    public void RetirarEstoque(int quantidade, Guid usuarioCriacaoId)
    {
        ValidarQuantidadeDisponivel(quantidade);

        if (quantidade > QuantidadeDisponivel)
            throw new InvalidOperationException("Não há estoque suficiente para retirar a quantidade solicitada.");

        QuantidadeDisponivel -= quantidade;
        AdicionarMovimentacao(quantidade, "Retirada de estoque", ETipoMovimentacao.Saida, usuarioCriacaoId, DateTime.UtcNow);
    }

    public void ReservarEstoque(int quantidade, Guid usuarioCriacaoId) 
    {
        ValidarQuantidadeReservada(quantidade);

        if (quantidade > QuantidadeDisponivel)
        {
            Console.WriteLine("Solicitação de compra criada e realizada com sucesso!");//mock de requisicao de compra
            AdicionarEstoque(quantidade, usuarioCriacaoId);
        }
        QuantidadeDisponivel -= quantidade;
        QuantidadeReservada += quantidade;
    }
    public void LiberarReserva(int quantidade, Guid usuarioCriacaoId) 
    {
        ValidarQuantidadeReservada(quantidade);

        if(quantidade > QuantidadeReservada)
            Console.WriteLine("Não há estoque reservado suficiente para liberar a quantidade solicitada.");
        
       
        QuantidadeReservada -= quantidade;
        QuantidadeDisponivel += quantidade;
    }

    private void AdicionarMovimentacao(int quantidade, string observacao, ETipoMovimentacao tipoMovimentacao, Guid UsuarioCriacaoId, DateTime dataCriacao)
    {
        var historico = new EstoqueHistorico(quantidade, observacao, tipoMovimentacao, UsuarioCriacaoId, dataCriacao, Id);
        Historicos.Add(historico);
    }
}

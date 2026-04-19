namespace Domain.Aggregates.Estoque;
public class EstoqueEntity : Base.BaseEntity
{
    public Guid PecaId { get; private set; }
    public int QuantidadeDisponivel { get; private set; }
    public int QuantidadeReservada { get; private set; }
    public int QuantidadeTotal => QuantidadeDisponivel + QuantidadeReservada;
    public EstoqueEntity(Guid pecaId, int quantidadeDisponivel, Guid UsuarioCriacaoId, DateTime dataCriacao) : base(UsuarioCriacaoId, dataCriacao, null, null)
    {
        ValidarPecaId(pecaId);
        ValidarQuantidadeDisponivel(quantidadeDisponivel);

        PecaId = pecaId;
        QuantidadeDisponivel = quantidadeDisponivel;
        QuantidadeReservada = 0;
    }

    protected EstoqueEntity() { }

    private void ValidarPecaId(Guid pecaId)
    {
        if (pecaId == Guid.Empty)
            throw new ArgumentException("O ID da peça não pode ser vazio.");
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

    public void AdicionarEstoque(int quantidade, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
    {
        ValidarQuantidadeDisponivel(quantidade);

        QuantidadeDisponivel += quantidade;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = dataAtualizacao;
    }

    public void RetirarEstoque(int quantidade, Guid idUsuarioAtualizacao, DateTime dataAtualizacao)
    {
        ValidarQuantidadeDisponivel(quantidade);

        if (quantidade > QuantidadeDisponivel)
            throw new InvalidOperationException("Não há estoque suficiente para retirar a quantidade solicitada.");
        
        QuantidadeDisponivel -= quantidade;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = dataAtualizacao;
    }

    public void ReservarEstoque(int quantidade, Guid idUsuarioAtualizacao, DateTime dataAtualizacao) 
    {
        ValidarQuantidadeReservada(quantidade);

        if(quantidade > QuantidadeDisponivel)
            throw new InvalidOperationException("Não há estoque suficiente para reservar a quantidade solicitada.");
        
        QuantidadeDisponivel -= quantidade;
        QuantidadeReservada += quantidade;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = dataAtualizacao;
    }
    public void LiberarReserva(int quantidade, Guid idUsuarioAtualizacao, DateTime dataAtualizacao) 
    {
        ValidarQuantidadeReservada(quantidade);
        if(quantidade > QuantidadeReservada)
            throw new InvalidOperationException("Não há estoque reservado suficiente para liberar a quantidade solicitada.");
       
        QuantidadeReservada -= quantidade;
        QuantidadeDisponivel += quantidade;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = dataAtualizacao;
    }
}

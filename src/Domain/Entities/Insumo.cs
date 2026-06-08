using Domain.BaseEntity;
using Domain.ValueObjects;

public class Insumo : Base
{
    public Insumo(string nome, string descricao, decimal custoUnitario, Guid usuarioCriacaoId, DateTime dataCriacao) : base(usuarioCriacaoId, dataCriacao, null, null)
    {
        ValidarNome(nome);
        ValidarDescricao(descricao);
        ValidarCusto(custoUnitario);

        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
        CustoUnitario = custoUnitario;
    }

    public Insumo(Guid id, string nome, string descricao, decimal custoUnitario,Guid idUsuarioCriacao, DateTime dataCriacao): base(idUsuarioCriacao, dataCriacao, null, null)
    {
        ValidarNome(nome);
        ValidarDescricao(descricao);
        ValidarCusto(custoUnitario);

        Id = id;
        Nome = nome;
        Descricao = descricao;
        CustoUnitario = custoUnitario;
    }

    public Insumo(Guid id, string nome, string descricao, decimal custoUnitario)
    {
        ValidarNome(nome);
        ValidarDescricao(descricao);
        ValidarCusto(custoUnitario);

        Id = id;
        Nome = nome;
        Descricao = descricao;
        CustoUnitario = custoUnitario;
    }

    protected Insumo() { }

    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal CustoUnitario { get; private set; }
    public ICollection<OrdemServicoInsumo> OrdemServicoInsumos { get; private set;  } = new List<OrdemServicoInsumo>();

    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do insumo é obrigatório.");
    }

    private void ValidarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição do insumo é obrigatória.");
    }

    private void ValidarCusto(decimal custo)
    {
        if (custo < 0)
            throw new ArgumentException("O custo não pode ser negativo.");
    }

    public void AtualizarNome(string nome)
    {
        ValidarNome(nome);
        Nome = nome;
    }

    public void AtualizarDescricao(string descricao)
    {
        ValidarDescricao(descricao);
        Descricao = descricao;
    }

    public void AtualizarCusto(decimal custo)
    {
        ValidarCusto(custo);
        CustoUnitario = custo;
    }
}
namespace Infra.DbModel;

public class InsumoDbModel
{
    public InsumoDbModel(Guid id, string nome, string descricao, decimal custoUnitario, Guid idUsuarioCriacao, DateTime dataCriacao, Guid? idUsuarioAtualizacao, DateTime? dataAtualizacao, bool ativo)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
        CustoUnitario = custoUnitario;
        IdUsuarioCriacao = idUsuarioCriacao;
        DataCriacao = dataCriacao;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = dataAtualizacao;
        Ativo = ativo;
    }

    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal CustoUnitario { get; set; }
    public Guid IdUsuarioCriacao { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid? IdUsuarioAtualizacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Ativo { get; set; }
    public ICollection<OrdemServicoInsumoDbModel> OrdemServicoInsumos { get; set; } = new List<OrdemServicoInsumoDbModel>();
}

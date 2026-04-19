using Domain.Enums;

namespace Domain.Aggregates.OrdemServico;
public class OrdemServicoEntity : Base.BaseEntity
{
    public Guid ClienteId { get; private set; }

    public Guid VeiculoId { get; private set; }

    public EStatusOS StatusOS { get; private set; }

    public string? Observacao { get; private set; }

    public decimal ValorTotal { get; private set; }

    public decimal ValorDesconto { get; private set; } = 0;

    private readonly List<OsServicoEntity> _servicos;
    private readonly List<OsPecaEntity> _pecas;
    protected OrdemServicoEntity() 
    {
        _servicos = new List<OsServicoEntity>();
        _pecas = new List<OsPecaEntity>();
    }

    public IReadOnlyCollection<OsServicoEntity> Servicos => _servicos.AsReadOnly();
    public IReadOnlyCollection<OsPecaEntity> Pecas => _pecas.AsReadOnly();

    public OrdemServicoEntity(Guid clienteId, Guid veiculoId,Guid idUsuarioCriacao) : base(idUsuarioCriacao, DateTime.UtcNow, null, null)
    {
        if(clienteId == Guid.Empty) throw new ArgumentException("O cliente é obrigatório.", nameof(clienteId));
        if(veiculoId == Guid.Empty) throw new ArgumentException("O veículo é obrigatório.", nameof(veiculoId));

        ClienteId = clienteId;
        VeiculoId = veiculoId;
        StatusOS = EStatusOS.Recebida;
        ValorDesconto = 0;
        ValorTotal = 0;
        IdUsuarioCriacao = idUsuarioCriacao;
        DataCriacao = DataCriacao;

        _servicos = new List<OsServicoEntity>();
        _pecas = new List<OsPecaEntity>();
    }

    public void IniciarDiagnostico(Guid idUsuarioAtualizacao)
    {
        ValidarTransicao(EStatusOS.Recebida, EStatusOS.EmDiagnostico);
        StatusOS = EStatusOS.EmDiagnostico;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void RegistrarDiagnostico(string observacao, Guid idUsuarioAtualizacao) 
    {

        if(StatusOS != EStatusOS.EmDiagnostico)
            throw new InvalidOperationException("A OS precisa estar Em Diagnóstico para registrar observações.");

        if(string.IsNullOrWhiteSpace(observacao)) throw new InvalidOperationException("Observação é obrigatória para registrar um diagnóstico.");

        Observacao = observacao;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void EnviarParaAprovacao(Guid idUsuarioAtualizacao)
    {
        ValidarTransicao(EStatusOS.EmDiagnostico, EStatusOS.AguardandoAprovacao);

        if(!_servicos.Any() && !_pecas.Any())
            throw new InvalidOperationException("A OS deve ter ao menos um serviço ou peça antes de enviar para aprovação.");

        RecalcularValorTotal();

        StatusOS = EStatusOS.AguardandoAprovacao;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AorovarOrdemServico(Guid idUsuarioAtualizacao)
    {
        ValidarTransicao(EStatusOS.AguardandoAprovacao, EStatusOS.EmExecucao);
        StatusOS = EStatusOS.EmExecucao;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void RecusarOrdemServico(Guid idUsuarioAtualizacao)
    {
        if (StatusOS != EStatusOS.AguardandoAprovacao)
            throw new InvalidOperationException("Só é possível recusar uma OS em Aguardando Aprovação.");

        StatusOS = EStatusOS.Cancelada;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void FinalizarOrdemServico(Guid idUsuarioAtualizacao)
    {
        ValidarTransicao(EStatusOS.EmExecucao, EStatusOS.Finalizada);
        StatusOS = EStatusOS.Finalizada;
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Entregar(Guid idUsuario, Guid idUsuarioAtualizacao)
    {
        ValidarTransicao(EStatusOS.Finalizada, EStatusOS.Entregue);
        StatusOS = EStatusOS.Entregue;
        IdUsuarioAtualizacao = idUsuario;
        DataAtualizacao = DateTime.UtcNow;
    }

    private void RecalcularValorTotal()
    {
        var totalServicos = _servicos.Sum(s => s.Valor);
        var totalPecas = _pecas.Sum(p => p.ValorUnitario * p.Quantidade);
        ValorTotal = totalServicos + totalPecas - ValorDesconto;
    }

    public void AdicionarServico(OsServicoEntity osServico, Guid idUsuarioAtualizacao)
    {
        if (osServico == null)
            throw new ArgumentNullException(nameof(osServico));

        ValidarStatusParaEdicao();

        bool jaExiste = _servicos.Any(s => s.ServicoId == osServico.ServicoId);
        if (jaExiste)
            throw new InvalidOperationException("Este serviço já foi adicionado à OS.");

        _servicos.Add(osServico);
        RecalcularValorTotal();
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void RemoverServico(Guid osServicoId, Guid idUsuarioAtualizacao)
    {
        ValidarStatusParaEdicao();
        
        var servico = _servicos.FirstOrDefault(s => s.ServicoId == osServicoId) ?? throw new InvalidOperationException("Serviço não encontrado na OS.");

        _servicos.Remove(servico);
        RecalcularValorTotal();
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AdicionarPeca(OsPecaEntity osPeca, Guid idUsuarioAtualizacao)
    {
        if (osPeca == null)
            throw new ArgumentNullException(nameof(osPeca));

        ValidarStatusParaEdicao();

        var pecaExistente = _pecas.FirstOrDefault(p => p.PecaId == osPeca.PecaId);
        if (pecaExistente != null)
            throw new InvalidOperationException("Esta peça já foi adicionada à OS. Atualize a quantidade.");

        _pecas.Add(osPeca);
        RecalcularValorTotal();
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void RemoverPeca(Guid pecaId, Guid idUsuarioAtualizacao) 
    {
        ValidarStatusParaEdicao();

        var peca = _pecas.FirstOrDefault(p => p.PecaId == pecaId)
            ?? throw new InvalidOperationException("Peça não encontrada na OS.");

        _pecas.Remove(peca);
        RecalcularValorTotal();
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }


    public void AplicarDesconto(decimal valorDesconto, Guid idUsuarioAtualizacao)
    {
        if (valorDesconto < 0)
            throw new ArgumentException("Valor de desconto não pode ser negativo.");

        if(valorDesconto > ValorTotal)
            throw new ArgumentException("Valor de desconto não pode ser maior que o valor total da OS.");

        ValorDesconto = valorDesconto;
        RecalcularValorTotal();
        IdUsuarioAtualizacao = idUsuarioAtualizacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    private void ValidarStatusParaEdicao()
    {
        var statusEditaveis = new[]
        {
            EStatusOS.Recebida,
            EStatusOS.EmDiagnostico
        };

        if (!statusEditaveis.Contains(StatusOS))
            throw new InvalidOperationException(
                $"Não é possível editar itens de uma OS com status '{StatusOS}'.");
    }

    private void ValidarTransicao(EStatusOS statusEsperado, EStatusOS proximoStatus)
    {
        if (StatusOS != statusEsperado)
            throw new InvalidOperationException(
                $"Transição inválida: OS está '{StatusOS}', esperado '{statusEsperado}' para ir para '{proximoStatus}'.");
    }
}

using Application.Estoques.DTOs.Requests;
using Application.Gateways.Estoques;
using Domain.Enums;

namespace Application.UseCases.Estoques;
public class MovimentarUseCase
{
    private readonly EstoqueGateway _estoqueGateway;

    public MovimentarUseCase(EstoqueGateway estoqueGateway)
    {
        _estoqueGateway = estoqueGateway;
    }

    public static MovimentarUseCase Create(EstoqueGateway estoqueGateway)
    {
        return new MovimentarUseCase(estoqueGateway);
    }

    public async Task<Guid> Run(EstoqueRequestDTO request, Guid idUsuario, CancellationToken ct)
    {
        if (request.InsumoId is null && request.PecaId is null)
            throw new ArgumentException("É obrigatório ter a PecaId ou o InsumoId preenchidos.");

        if (request.InsumoId is not null && request.PecaId is not null)
            throw new ArgumentException("Apenas uma opção deve ser preenchida: PecaId ou InsumoId.");

        if (!Enum.TryParse<ETipoMovimentacao>(request.TipoMovimentacao, true, out var tipoMovimentacao))
            throw new ArgumentException("Tipo de movimentação inválido.");

        var estoque = request.InsumoId is not null
            ? await _estoqueGateway.GetByInsumoId(request.InsumoId.Value, ct)
            : await _estoqueGateway.GetByPecaId(request.PecaId!.Value, ct);

        if (estoque is null)
            throw new KeyNotFoundException("Estoque não encontrado para o Insumo ou Peça informado.");

        if (tipoMovimentacao == ETipoMovimentacao.Entrada)
            estoque.AdicionarEstoque(request.Quantidade, idUsuario);
        else
            estoque.RetirarEstoque(request.Quantidade, idUsuario);

        estoque.RastrearAlteracao(idUsuario, DateTime.UtcNow);

        await _estoqueGateway.Update(estoque, ct);

        return estoque.Id;
    }
}

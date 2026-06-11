using Application.Interfaces;
using Domain.Aggregates.EstoqueAggregates;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Estoques.Input;

namespace Infra.DataSources;

public class EstoqueDataSource : IEstoqueDataSource
{
    private readonly AppDbContext _appDbContext;

    public EstoqueDataSource(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<(List<EstoqueInputDTO> estoques, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Estoque> query = _appDbContext.Estoques.Where(e => e.Ativo);

        var total = await query.CountAsync(ct);

        var estoques = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        var estoquesResponse = estoques.Select(e => new EstoqueInputDTO
        {
            Id = e.Id,
            PecaId = e.PecaId,
            InsumoId = e.InsumoId,
            QuantidadeDisponivel = e.QuantidadeDisponivel,
            QuantidadeReservada = e.QuantidadeReservada,
            IdUsuarioCriacao = e.UsuarioCriacaoId,
            DataCriacao = e.DataCriacao
        }).ToList();

        return (estoquesResponse, total);
    }

    public async Task<EstoqueInputDTO?> GetById(Guid id, CancellationToken ct)
    {
        var estoque = await _appDbContext.Estoques
            .FirstOrDefaultAsync(e => e.Id == id && e.Ativo, ct);

        if (estoque == null)
            return null;

        return new EstoqueInputDTO
        {
            Id = estoque.Id,
            PecaId = estoque.PecaId,
            InsumoId = estoque.InsumoId,
            QuantidadeDisponivel = estoque.QuantidadeDisponivel,
            QuantidadeReservada = estoque.QuantidadeReservada,
            IdUsuarioCriacao = estoque.UsuarioCriacaoId,
            DataCriacao = estoque.DataCriacao
        };
    }

    public async Task<EstoqueInputDTO?> GetByInsumoId(Guid insumoId, CancellationToken ct)
    {
        var estoque = await _appDbContext.Estoques
            .FirstOrDefaultAsync(e => e.InsumoId == insumoId && e.Ativo, ct);

        if (estoque == null)
            return null;

        return new EstoqueInputDTO
        {
            Id = estoque.Id,
            PecaId = estoque.PecaId,
            InsumoId = estoque.InsumoId,
            QuantidadeDisponivel = estoque.QuantidadeDisponivel,
            QuantidadeReservada = estoque.QuantidadeReservada,
            IdUsuarioCriacao = estoque.UsuarioCriacaoId,
            DataCriacao = estoque.DataCriacao
        };
    }

    public async Task<EstoqueInputDTO?> GetByPecaId(Guid pecaId, CancellationToken ct)
    {
        var estoque = await _appDbContext.Estoques
            .FirstOrDefaultAsync(e => e.PecaId == pecaId && e.Ativo, ct);

        if (estoque == null)
            return null;

        return new EstoqueInputDTO
        {
            Id = estoque.Id,
            PecaId = estoque.PecaId,
            InsumoId = estoque.InsumoId,
            QuantidadeDisponivel = estoque.QuantidadeDisponivel,
            QuantidadeReservada = estoque.QuantidadeReservada,
            IdUsuarioCriacao = estoque.UsuarioCriacaoId,
            DataCriacao = estoque.DataCriacao
        };
    }

    public async Task Update(EstoqueInputDTO request, CancellationToken ct)
    {
        var entity = await _appDbContext.Estoques
            .Include(e => e.Historicos)
            .FirstOrDefaultAsync(e => e.Id == request.Id && e.Ativo, ct);

        if (entity == null)
            throw new Exception("Estoque não encontrado");

        _appDbContext.Entry(entity).Property(e => e.QuantidadeDisponivel).CurrentValue = request.QuantidadeDisponivel;
        _appDbContext.Entry(entity).Property(e => e.QuantidadeReservada).CurrentValue = request.QuantidadeReservada;

        foreach (var historico in request.Historicos)
        {
            var novoHistorico = new EstoqueHistorico(
                historico.Quantidade,
                historico.Observacao,
                Enum.Parse<Domain.Enums.ETipoMovimentacao>(historico.TipoMovimentacao),
                historico.IdUsuarioCriacao,
                historico.DataCriacao,
                historico.EstoqueId
            );
            entity.Historicos.Add(novoHistorico);
        }

        entity.RastrearAlteracao(request.IdUsuarioAtualizacao ?? Guid.Empty, DateTime.UtcNow);

        await _appDbContext.SaveChangesAsync(ct);
    }
}
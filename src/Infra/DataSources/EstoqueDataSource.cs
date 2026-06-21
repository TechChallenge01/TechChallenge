using Application.Interfaces;
using Domain.Aggregates.EstoqueAggregates;
using Infra.Context;
using Infra.DbModel;
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
        IQueryable<EstoqueDbModel> query = _appDbContext.Estoques.Where(e => e.Ativo);

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
            Historicos = e.Historicos.Select(h => new EstoqueHistoricoInputDTO
            {
                DataCriacao = h.DataCriacao,
                EstoqueId = h.EstoqueId,
                Id = h.Id,
                IdUsuarioCriacao = h.IdUsuarioCriacao,
                Observacao = h.Observacao,
                Quantidade = h.Quantidade,
                TipoMovimentacao = h.TipoMovimentacao
            }).ToList()
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
            Historicos = estoque.Historicos.Select(h => new EstoqueHistoricoInputDTO
            {
                DataCriacao = h.DataCriacao,
                EstoqueId = h.EstoqueId,
                Id = h.Id,
                IdUsuarioCriacao = h.IdUsuarioCriacao,
                Observacao = h.Observacao,
                Quantidade = h.Quantidade,
                TipoMovimentacao = h.TipoMovimentacao
            }).ToList()
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
            Historicos = estoque.Historicos.Select(h => new EstoqueHistoricoInputDTO
            {
                DataCriacao = h.DataCriacao,
                EstoqueId = h.EstoqueId,
                Id = h.Id,
                IdUsuarioCriacao = h.IdUsuarioCriacao,
                Observacao = h.Observacao,
                Quantidade = h.Quantidade,
                TipoMovimentacao = h.TipoMovimentacao
            }).ToList()
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
            Historicos = estoque.Historicos.Select(h => new EstoqueHistoricoInputDTO
            {
                DataCriacao = h.DataCriacao,
                EstoqueId = h.EstoqueId,
                Id = h.Id,
                IdUsuarioCriacao = h.IdUsuarioCriacao,
                Observacao = h.Observacao,
                Quantidade = h.Quantidade,
                TipoMovimentacao = h.TipoMovimentacao
            }).ToList()
        };
    }

    public async Task Update(EstoqueInputDTO request, CancellationToken ct)
    {
        var estoqueDbModel = await _appDbContext.Estoques
            .Include(e => e.Historicos)
            .FirstOrDefaultAsync(e => e.Id == request.Id, ct);

        if (estoqueDbModel == null)
            throw new Exception("Estoque não encontrado");

        estoqueDbModel.QuantidadeDisponivel = request.QuantidadeDisponivel;
        estoqueDbModel.QuantidadeReservada = request.QuantidadeReservada;
        estoqueDbModel.Historicos = request.Historicos.Select(h => new EstoqueHistoricoDbmodel(h.Id, h.Quantidade, h.Observacao, h.TipoMovimentacao, request.Id, h.IdUsuarioCriacao, h.DataCriacao)).ToList();

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<List<EstoqueInputDTO>?> GetByPecasIds(ICollection<Guid> ids, CancellationToken ct)
    {
        var estoque = await _appDbContext.Estoques.Where(e => ids.Contains((Guid)e.PecaId) && e.Ativo).ToListAsync(ct);

        if (estoque == null)
            return null;

        return estoque.Select(e => new EstoqueInputDTO
        {
            Id = e.Id,
            PecaId = e.PecaId,
            InsumoId = e.InsumoId,
            QuantidadeDisponivel = e.QuantidadeDisponivel,
            QuantidadeReservada = e.QuantidadeReservada,
            Historicos = e.Historicos.Select(h => new EstoqueHistoricoInputDTO
            {
                DataCriacao = h.DataCriacao,
                EstoqueId = h.EstoqueId,
                Id = h.Id,
                IdUsuarioCriacao = h.IdUsuarioCriacao,
                Observacao = h.Observacao,
                Quantidade = h.Quantidade,
                TipoMovimentacao = h.TipoMovimentacao
            }).ToList()
        }).ToList();
    }

    public async Task<List<EstoqueInputDTO>?> GetByInsumosIds(ICollection<Guid> ids, CancellationToken ct)
    {
        var estoque = await _appDbContext.Estoques.Where(e => ids.Contains((Guid)e.InsumoId) && e.Ativo).ToListAsync(ct);

        if (estoque == null)
            return null;

        return estoque.Select(e => new EstoqueInputDTO
        {
            Id = e.Id,
            PecaId = e.PecaId,
            InsumoId = e.InsumoId,
            QuantidadeDisponivel = e.QuantidadeDisponivel,
            QuantidadeReservada = e.QuantidadeReservada,
            Historicos = e.Historicos.Select(h => new EstoqueHistoricoInputDTO
            {
                DataCriacao = h.DataCriacao,
                EstoqueId = h.EstoqueId,
                Id = h.Id,
                IdUsuarioCriacao = h.IdUsuarioCriacao,
                Observacao = h.Observacao,
                Quantidade = h.Quantidade,
                TipoMovimentacao = h.TipoMovimentacao
            }).ToList()
        }).ToList();
    }

    public async Task UpdateEstoques(ICollection<EstoqueInputDTO> estoques, CancellationToken ct)
    {

        var ids = estoques.Select(e => e.Id).ToList();
        var estoquesDbModel = await _appDbContext.Estoques
            .Include(e => e.Historicos)
            .Where(e => ids.Contains(e.Id)).ToListAsync(ct);

        if (estoques.Count() != estoquesDbModel.Count())
            throw new Exception("Um ou mais estoques não encontrados");

        EstoqueInputDTO estoqueRequest;

        foreach(var estoque in estoquesDbModel)
        {
            estoqueRequest = estoques.FirstOrDefault(e => e.Id == estoque.Id);

            estoque.QuantidadeDisponivel = estoqueRequest.QuantidadeDisponivel;
            estoque.QuantidadeReservada = estoqueRequest.QuantidadeReservada;
            estoque.Historicos = estoqueRequest.Historicos.Select(h => new EstoqueHistoricoDbmodel(h.Id, h.Quantidade, h.Observacao, h.TipoMovimentacao, estoqueRequest.Id, h.IdUsuarioCriacao, h.DataCriacao)).ToList();
        }
        

        await _appDbContext.SaveChangesAsync(ct);
    }
}
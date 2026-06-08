using Application.Interfaces;
using Domain.Entities;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Pecas.Input;

namespace Infra.DataSources;

public class PecaDataSource : IPecaDataSource
{
    private readonly AppDbContext _appDbContext;

    public PecaDataSource(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Create(PecaInputDTO peca, CancellationToken ct)
    {
        var entity = new Peca(peca.Id, peca.Nome, peca.Descricao, peca.MarcaPeca, peca.ValorUnitario,
            peca.IdUsuarioCriacao, peca.DataCriacao);

        await _appDbContext.Pecas.AddAsync(entity, ct);
        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var entity = await _appDbContext.Pecas
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo, ct);

        if (entity == null)
            throw new Exception("Peça não encontrada");

        entity.Inativar();
        entity.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<PecaInputDTO?> GetById(Guid id, CancellationToken ct)
    {
        var peca = await _appDbContext.Pecas
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo, ct);

        if (peca == null)
            return null;

        return new PecaInputDTO
        {
            Id = peca.Id,
            Nome = peca.Nome,
            Descricao = peca.Descricao,
            MarcaPeca = peca.MarcaPeca,
            ValorUnitario = peca.ValorUnitario
        };
    }

    public async Task<(List<PecaInputDTO> pecas, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Peca> query = _appDbContext.Pecas.Where(p => p.Ativo);

        var total = await query.CountAsync(ct);

        var pecas = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        var pecasResponse = pecas.Select(p => new PecaInputDTO
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            MarcaPeca = p.MarcaPeca,
            ValorUnitario = p.ValorUnitario
        }).ToList();

        return (pecasResponse, total);
    }

    public async Task Update(PecaInputDTO request, CancellationToken ct)
    {
        var entity = await _appDbContext.Pecas
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.Ativo, ct);

        if (entity == null)
            throw new Exception("Peça não encontrada");

        if (!request.Ativo)
            entity.Inativar();

        entity.AlterarNome(request.Nome);
        entity.AlterarDescricao(request.Descricao);
        entity.AlterarMarcaPeca(request.MarcaPeca);
        entity.AlterarPrecoVenda(request.ValorUnitario);
        entity.RastrearAlteracao(request.IdUsuarioAtualizacao ?? Guid.Empty, DateTime.UtcNow);

        await _appDbContext.SaveChangesAsync(ct);
    }
}
using Application.Interfaces;
using Domain.Entities;
using Infra.Context;
using Infra.DbModel;
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
        var pecaDbModel = new PecaDbModel(peca.Id, peca.Nome, peca.Descricao, peca.MarcaPeca, peca.ValorUnitario, peca.IdUsuarioCriacao, peca.DataCriacao, peca.IdUsuarioAtualizacao, peca.DataAtualizacao, peca.Ativo);

        await _appDbContext.Pecas.AddAsync(pecaDbModel, ct);
        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<PecaInputDTO?> GetById(Guid id, CancellationToken ct)
    {
        var peca = await _appDbContext.Pecas.FirstOrDefaultAsync(p => p.Id == id && p.Ativo, ct);

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

    public async Task<List<PecaInputDTO>?> GetByIds(List<Guid> ids, CancellationToken ct)
    {
        var pecas = await _appDbContext.Pecas.Where(p => ids.Contains(p.Id)).ToListAsync(ct);

        if (pecas == null)
            return null;

        return pecas.Select(p => new PecaInputDTO
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            MarcaPeca = p.MarcaPeca,
            ValorUnitario = p.ValorUnitario
        }).ToList();
    }

    public async Task<(List<PecaInputDTO> pecas, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<PecaDbModel> query = _appDbContext.Pecas.Where(p => p.Ativo);

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
        var pecaDbModel = await _appDbContext.Pecas.FirstOrDefaultAsync(p => p.Id == request.Id && p.Ativo, ct);

        if (pecaDbModel == null)
            throw new Exception("Peça não encontrada");


        pecaDbModel.Nome = request.Nome;
        pecaDbModel.Descricao = request.Descricao;
        pecaDbModel.MarcaPeca = request.MarcaPeca;
        pecaDbModel.ValorUnitario = request.ValorUnitario;
        pecaDbModel.DataAtualizacao = request.DataAtualizacao;
        pecaDbModel.IdUsuarioAtualizacao = request.IdUsuarioAtualizacao;
        pecaDbModel.Ativo = pecaDbModel.Ativo;

        await _appDbContext.SaveChangesAsync(ct);
    }
}
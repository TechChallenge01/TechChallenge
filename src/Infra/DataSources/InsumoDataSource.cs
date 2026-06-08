using Application.Interfaces;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Insumo.Input;

namespace Infra.DataSources;

public class InsumoDataSource : IInsumoDataSource
{
    private readonly AppDbContext _appDbContext;

    public InsumoDataSource(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Create(InsumoInputDTO insumo, CancellationToken ct)
    {
        var entity = new Insumo(insumo.Nome, insumo.Descricao, insumo.CustoUnitario,
            insumo.IdUsuarioCriacao, insumo.DataCriacao);

        await _appDbContext.Insumos.AddAsync(entity, ct);
        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var entity = await _appDbContext.Insumos
            .FirstOrDefaultAsync(i => i.Id == id && i.Ativo, ct);

        if (entity == null)
            throw new Exception("Insumo não encontrado");

        entity.Inativar();
        entity.RastrearAlteracao(Guid.Empty, DateTime.UtcNow);

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<InsumoInputDTO?> GetById(Guid id, CancellationToken ct)
    {
        var insumo = await _appDbContext.Insumos
            .FirstOrDefaultAsync(i => i.Id == id && i.Ativo, ct);

        if (insumo == null)
            return null;

        return new InsumoInputDTO
        {
            Id = insumo.Id,
            Nome = insumo.Nome,
            Descricao = insumo.Descricao,
            CustoUnitario = insumo.CustoUnitario,
            IdUsuarioCriacao = insumo.IdUsuarioCriacao,
            DataCriacao = insumo.DataCriacao
        };
    }

    public async Task<(List<InsumoInputDTO> insumos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Insumo> query = _appDbContext.Insumos.Where(i => i.Ativo);

        var insumos = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        var insumosResponse = insumos.Select(i => new InsumoInputDTO
        {
            Id = i.Id,
            Nome = i.Nome,
            Descricao = i.Descricao,
            CustoUnitario = i.CustoUnitario,
            IdUsuarioCriacao = i.IdUsuarioCriacao,
            DataCriacao = i.DataCriacao
        }).ToList();

        var total = await query.CountAsync(ct);

        return (insumosResponse, total);
    }

    public async Task Update(InsumoInputDTO request, CancellationToken ct)
    {
        var entity = await _appDbContext.Insumos
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.Ativo, ct);

        if (entity == null)
            throw new Exception("Insumo não encontrado");

        entity.AtualizarNome(request.Nome);
        entity.AtualizarDescricao(request.Descricao);
        entity.AtualizarCusto(request.CustoUnitario);
        entity.RastrearAlteracao(request.IdUsuarioAtualizacao ?? Guid.Empty, DateTime.UtcNow);

        await _appDbContext.SaveChangesAsync(ct);
    }
}

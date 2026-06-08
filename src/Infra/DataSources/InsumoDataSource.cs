using Application.Interfaces;
using Infra.Context;
using Infra.DbModel;
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
        var insumoDbModel = new InsumoDbModel(insumo.Id, insumo.Nome, insumo.Descricao, insumo.CustoUnitario,
            insumo.IdUsuarioCriacao, insumo.DataCriacao, insumo.IdUsuarioAtualizacao, insumo.DataAtualizacao);
        
        await _appDbContext.Insumos.AddAsync(insumoDbModel, ct);
        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var insumoDbModel = await _appDbContext.Insumos
            .FirstOrDefaultAsync(i => i.Id == id && i.Ativo, ct);

        if (insumoDbModel == null)
            throw new Exception("Insumo não encontrado");

        insumoDbModel.Ativo = false;
        insumoDbModel.DataAtualizacao = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<InsumoInputDTO> GetById(Guid id, CancellationToken ct)
    {
        var insumo = await _appDbContext.Insumos.FirstOrDefaultAsync(i => i.Id == id && i.Ativo, ct);

        if (insumo == null)
            return null;

        return new InsumoInputDTO
        {
            Id = insumo.Id,
            Nome = insumo.Nome,
            Descricao = insumo.Descricao,
            CustoUnitario = insumo.CustoUnitario
        };
    }

    public async Task<(List<InsumoInputDTO> insumos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<InsumoDbModel> query = _appDbContext.Insumos.Where(i => i.Ativo);

        var insumos = await query.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsNoTracking()
                                 .ToListAsync();

        var insumosResponse = insumos.Select(i => new InsumoInputDTO
        {
            Id = i.Id,
            Nome = i.Nome,
            Descricao = i.Descricao,
            CustoUnitario = i.CustoUnitario
        }).ToList();

        var total = await query.CountAsync(ct);

        return (insumosResponse, total);    
    }

    public async Task Update(InsumoInputDTO request, CancellationToken ct)
    {
        var insumoDbModel = await _appDbContext.Insumos.FirstOrDefaultAsync(i => i.Id == request.Id && i.Ativo, ct);

        if (insumoDbModel == null)
            throw new Exception("Insumo não encontrado");

        insumoDbModel.Nome = request.Nome;
        insumoDbModel.Descricao = request.Descricao;
        insumoDbModel.CustoUnitario = request.CustoUnitario;
        insumoDbModel.IdUsuarioAtualizacao = request.IdUsuarioAtualizacao;
        insumoDbModel.DataAtualizacao = request.DataAtualizacao;

        await _appDbContext.SaveChangesAsync(ct);
    }
}

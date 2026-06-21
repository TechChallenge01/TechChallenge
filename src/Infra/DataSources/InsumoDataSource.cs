using Application.Interfaces;
using Infra.Context;
using Infra.DbModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Insumos.Input;

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
        var insumoDbModel = new InsumoDbModel(insumo.Id, insumo.Nome, insumo.Descricao, insumo.CustoUnitario,insumo.IdUsuarioCriacao, insumo.DataCriacao, insumo.IdUsuarioAtualizacao, insumo.DataAtualizacao, insumo.Ativo);

        await _appDbContext.Insumos.AddAsync(insumoDbModel, ct);
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
            CustoUnitario = insumo.CustoUnitario
        };
    }

    public async Task<List<InsumoInputDTO>> GetByIds(List<Guid> ids, CancellationToken cancellationToken)
    {
        var insumos = await _appDbContext.Insumos.Where(i => ids.Contains(i.Id) && i.Ativo).ToListAsync(cancellationToken);

        if (insumos == null)
            return null;

        return insumos.Select(i => new InsumoInputDTO
        {
            Id = i.Id,
            Nome = i.Nome,
            Descricao = i.Descricao,
            CustoUnitario = i.CustoUnitario
        }).ToList();
    }

    public async Task<(List<InsumoInputDTO> insumos, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<InsumoDbModel> query = _appDbContext.Insumos.Where(i => i.Ativo);

        var total = await query.CountAsync(ct);

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
            CustoUnitario = i.CustoUnitario
        }).ToList();

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
        insumoDbModel.DataAtualizacao = request.DataAtualizacao;
        insumoDbModel.IdUsuarioAtualizacao = request.IdUsuarioAtualizacao;
        insumoDbModel.Ativo = request.Ativo;

        await _appDbContext.SaveChangesAsync(ct);
    }
}

using Application.Interfaces;
using Domain.Aggregates.EstoqueAggregates;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Tsp;
using Shared.DTOs.Estoques.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Gateways.Estoques;
public class EstoqueGateway
{
    private readonly IEstoqueDataSource _dataSource;

    private EstoqueGateway(IEstoqueDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public static EstoqueGateway Create(IEstoqueDataSource dataSource)
    {
        return new EstoqueGateway(dataSource);
    }

    public async Task Update(Estoque estoque, CancellationToken ct)
    {
        var estoqueDTO = new EstoqueInputDTO
        {
            Id = estoque.Id,
            PecaId = estoque.PecaId,
            InsumoId = estoque.InsumoId,
            QuantidadeDisponivel = estoque.QuantidadeDisponivel,
            QuantidadeReservada = estoque.QuantidadeReservada,
            IdUsuarioAtualizacao = estoque.IdUsuarioAtualizacao,
            DataAtualizacao = estoque.DataAtualizacao,
            Ativo = estoque.Ativo,
            Historicos = estoque.Historicos.Select(h => new EstoqueHistoricoInputDTO
            {
                Id = h.Id,
                Quantidade = h.Quantidade,
                Observacao = h.Observacao,
                TipoMovimentacao = h.TipoMovimentacao,
                EstoqueId = h.EstoqueId,
                IdUsuarioCriacao = h.IdUsuarioCriacao,
                DataCriacao = h.DataCriacao
            }).ToList()
        };

        await _dataSource.Update(estoqueDTO, ct);
    }

    public async Task<Estoque?> GetById(Guid id, CancellationToken ct)
    {
        var response = await _dataSource.GetById(id, ct);

        if (response == null)
            return null;

        return new Estoque(response.Id, response.InsumoId, response.PecaId, response.QuantidadeDisponivel, response.QuantidadeReservada, response.IdUsuarioCriacao, response.DataCriacao);
    }

    public async Task<(List<Estoque> Estoques, int total)> GetPaginated(int page, int pageSize, CancellationToken ct)
    {
        var response = await _dataSource.GetPaginated(page, pageSize, ct);

        var estoques = response.estoques
            .Select(e => new Estoque(e.Id, e.InsumoId, e.PecaId, e.QuantidadeDisponivel, e.QuantidadeReservada, e.IdUsuarioCriacao, e.DataCriacao))
            .ToList();

        return (estoques, response.total);
    }

    public async Task<Estoque?> GetByInsumoId(Guid insumoId, CancellationToken ct)
    {
        var response = await _dataSource.GetByInsumoId(insumoId, ct);

        if (response == null)
            return null;

        return new Estoque(response.Id, response.InsumoId, response.PecaId, response.QuantidadeDisponivel, response.QuantidadeReservada, response.IdUsuarioCriacao, response.DataCriacao);
    }

    public async Task<Estoque?> GetByPecaId(Guid pecaId, CancellationToken ct)
    {
        var response = await _dataSource.GetByPecaId(pecaId, ct);

        if (response == null)
            return null;

        return new Estoque(response.Id, response.InsumoId, response.PecaId, response.QuantidadeDisponivel, response.QuantidadeReservada, response.IdUsuarioCriacao, response.DataCriacao);
    }

}

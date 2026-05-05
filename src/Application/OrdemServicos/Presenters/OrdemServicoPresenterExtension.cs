using Application.OrdemServicos.DTOs.Responses;
using Domain.Aggregates.OrdemServicoAggregates;

namespace Application.OrdemServicos.Presenters;

public static class OrdemServicoPresenterExtension
{
    public static OrdemServicoResponseDTO ToDTO(this OrdemServico ordemServico) 
    {
        return new OrdemServicoResponseDTO
        {
            Id = ordemServico.Id,
            StatusOS = ordemServico.StatusOS.ToString(),
            Observacao = ordemServico.Observacao,
            ValorTotal = ordemServico.ValorTotal,
            ValorDesconto = ordemServico.ValorDesconto,
            TempoExecucao = ordemServico.TempoExecucao,
            Pecas = ordemServico.Pecas == null ? null : ordemServico.Pecas.Select(p => new OrdemServicoPecaResponseDTO
            {
                PecaId = p.PecaId,
                Quantidade = p.Quantidade,
                ValorUnitario = p.ValorUnitario,
                ValorTotal = p.ValorTotal
            }).ToList(),

            Servicos = ordemServico.Servicos == null ? null : ordemServico.Servicos.Select(s => new OrdemServicoServicoResponseDTO
            {
                ServicoId = s.ServicoId,
                Quantidade = s.Quantidade,
                ValorUnitario = s.ValorUnitario,
                ValorTotal = s.ValorTotal,
                StatusOS = s.Status
            }).ToList(),
            Insumos = ordemServico.Insumos == null ? null : ordemServico.Insumos.Select(i => new OrdemServicoInsumoResponseDTO
            {
                InsumoId = i.InsumoId,
                CustoTotal = i.ValorTotal,
                CustoUnitario = i.CustoUnitario,
                Quantidade = i.Quantidade
            }).ToList()
        };
    }

    public static ICollection<OrdemServicoResponseDTO> ToListDTO(this ICollection<OrdemServico> ordemServicos)
    {
        return ordemServicos.Select(os => os.ToDTO()).ToList();
    }
}

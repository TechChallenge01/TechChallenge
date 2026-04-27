using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdemServicoController : ControllerBase
{

    private readonly IOrdemServicoService _ordemService;

    public OrdemServicoController(IOrdemServicoService ordemService)
    {
        _ordemService = ordemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) 
    {
        var response = await _ordemService.GetPaginated(page, pageSize, ct);

        return response.ToResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrdemServicoRequestDTO request, CancellationToken ct)
    {
        var response = await _ordemService.Create(request, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/Cancelar")]
    public async Task<IActionResult> Cancelar([FromRoute] int id, CancellationToken ct)
    {
        var response = await _ordemService.Cancelar(id, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/Aprovar")]
    public async Task<IActionResult> Aprovar([FromRoute] int id, CancellationToken ct)
    {
        var response = await _ordemService.Aprovar(id, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/FinalizarServico")]
    public async Task<IActionResult> FinalizarServico([FromRoute] int id, [FromBody] FinalizarServicoDTO dto, CancellationToken ct)
    {
        var response = await _ordemService.FinalizarServico(id, dto, ct);

        return response.ToResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var response = await _ordemService.GetById(id, ct);

        return response.ToResult();
    }
}

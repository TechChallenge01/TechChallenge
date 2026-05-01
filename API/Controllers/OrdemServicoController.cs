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
    public async Task<IActionResult> Cancelar([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _ordemService.Cancelar(id, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/Aprovar")]
    public async Task<IActionResult> Aprovar([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _ordemService.Aprovar(id, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/FinalizarServico")]
    public async Task<IActionResult> FinalizarServico([FromRoute] Guid id, [FromBody] FinalizarServicoDTO dto, CancellationToken ct)
    {
        var response = await _ordemService.FinalizarServico(id, dto, ct);

        return response.ToResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _ordemService.GetById(id, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/IniciarDiagnostico")]
    public async Task<IActionResult> IniciarDiagnostico([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _ordemService.IniciarDiagnostico(id, ct);

        return response.ToResult();
    }
    [HttpPost("{id}/RealizarDiagnostico")]
    public async Task<IActionResult> RealizarDiagnostico([FromRoute] Guid id, [FromBody] DiagnosticoRequestDTO request, CancellationToken ct)
    {
        var response = await _ordemService.RealizarDiagnostico(id, request, ct);
        return response.ToResult();
    }

    [HttpPost("{id}/RegistrarEntrega")]
    public async Task<IActionResult> RegistrarEntrega([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _ordemService.RegistrarEntrega(id, ct);
        return response.ToResult();
    }
}

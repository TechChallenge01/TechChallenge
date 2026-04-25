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
}

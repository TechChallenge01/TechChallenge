using Application.Servicos.DTOs.Requests;
using Application.Servicos.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicoController : ControllerBase
{
    private readonly IServicoService _servicoService;
    public ServicoController(IServicoService servicoService)
    {
        _servicoService = servicoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var response = await _servicoService.GetPaginated(page, pageSize, ct);
        
        return response.ToResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ServicoRequestDTO request, CancellationToken ct) 
    {
        var response = await _servicoService.Create(request, ct);

        return response.ToResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _servicoService.Delete(id, ct);

        return response.ToResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] ServicoRequestDTO request, CancellationToken ct) 
    {
        var response = await _servicoService.Update(id, request, ct);

        return response.ToResult();
    }
}

using API.Extensions;
using Application.Servicos.DTOs.Requests;
using Application.Servicos.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Administrador,Funcionario,Mecanico")]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var response = await _servicoService.GetPaginated(page, pageSize, ct);
        
        return response.ToResult();
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Create([FromBody] ServicoRequestDTO request, CancellationToken ct) 
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _servicoService.Create(request, idUsuario, ct);

        return response.ToResult();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _servicoService.Delete(id, idUsuario, ct);

        return response.ToResult();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ServicoRequestDTO request, CancellationToken ct) 
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _servicoService.Update(id, idUsuario, request, ct);

        return response.ToResult();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Funcionario,Mecanico")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _servicoService.GetById(id, ct);

        return response.ToResult();
    }
}

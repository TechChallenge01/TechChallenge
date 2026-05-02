using API.Extensions;
using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Administrador,Funcionario,Mecanico")]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) 
    {
        var response = await _ordemService.GetPaginated(page, pageSize, ct);

        return response.ToResult();
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Create([FromBody] OrdemServicoRequestDTO request, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.Create(request, idUsuario, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/Cancelar")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Cancelar([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.Cancelar(id, idUsuario, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/Aprovar")]
    [Authorize(Roles = "Administrador,Funcionario,Cliente")]
    public async Task<IActionResult> Aprovar([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.Aprovar(id, idUsuario, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/FinalizarServico")]
    [Authorize(Roles = "Administrador,Mecanico")]
    public async Task<IActionResult> FinalizarServico([FromRoute] Guid id, [FromBody] FinalizarServicoDTO dto, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.FinalizarServico(id, idUsuario, dto, ct);

        return response.ToResult();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Funcionario,Mecanico,Cliente")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _ordemService.GetById(id, ct);

        return response.ToResult();
    }

    [HttpPost("{id}/IniciarDiagnostico")]
    [Authorize(Roles = "Administrador,Mecanico")]
    public async Task<IActionResult> IniciarDiagnostico([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.IniciarDiagnostico(id, idUsuario, ct);

        return response.ToResult();
    }
    [HttpPost("{id}/RealizarDiagnostico")]
    [Authorize(Roles = "Administrador,Mecanico")]
    public async Task<IActionResult> RealizarDiagnostico([FromRoute] Guid id, [FromBody] DiagnosticoRequestDTO request, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.RealizarDiagnostico(id, idUsuario, request, ct);
        return response.ToResult();
    }

    [HttpPost("{id}/RegistrarEntrega")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> RegistrarEntrega([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.RegistrarEntrega(id, idUsuario, ct);
        return response.ToResult();
    }
}

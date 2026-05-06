using API.Extensions;
using Application.OrdemServicos.DTOs.Requests;
using Application.OrdemServicos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers;

/// <summary>
/// Controller responsável por gerenciar ordens de serviço
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdemServicoController : ControllerBase
{

    private readonly IOrdemServicoService _ordemService;

    public OrdemServicoController(IOrdemServicoService ordemService)
    {
        _ordemService = ordemService;
    }

    /// <summary>
    /// Obtém lista paginada de ordens de serviço
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de ordens de serviço</returns>
    [HttpGet]
    [Authorize(Roles = "Administrador,Funcionario,Mecanico")]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) 
    {
        var response = await _ordemService.GetPaginated(page, pageSize, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Cria uma nova ordem de serviço
    /// </summary>
    /// <param name="request">Dados da ordem de serviço a ser criada</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>ID da ordem de serviço criada</returns>
    [HttpPost]
    [Authorize(Roles = "Administrador,Funcionario")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] OrdemServicoRequestDTO request, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.Create(request, idUsuario, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Cancela uma ordem de serviço
    /// </summary>
    /// <param name="id">ID da ordem de serviço a ser cancelada</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação de cancelamento</returns>
    [HttpPut("{id}/Cancelar")]
    [Authorize(Roles = "Administrador,Funcionario")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancelar([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.Cancelar(id, idUsuario, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Aprova uma ordem de serviço
    /// </summary>
    /// <param name="id">ID da ordem de serviço a ser aprovada</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação de aprovação</returns>
    [HttpPut("{id}/Aprovar")]
    [Authorize(Roles = "Administrador,Funcionario,Cliente")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Aprovar([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.Aprovar(id, idUsuario, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Finaliza o serviço de uma ordem
    /// </summary>
    /// <param name="id">ID da ordem de serviço</param>
    /// <param name="dto">Dados para finalizar o serviço</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação de finalização</returns>
    [HttpPut("{id}/FinalizarServico")]
    [Authorize(Roles = "Administrador,Mecanico")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FinalizarServico([FromRoute] Guid id, [FromBody] FinalizarServicoDTO dto, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.FinalizarServico(id, idUsuario, dto, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Obtém uma ordem de serviço por ID
    /// </summary>
    /// <param name="id">ID da ordem de serviço</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados da ordem de serviço</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Funcionario,Mecanico,Cliente")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _ordemService.GetById(id, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Inicia o diagnóstico de uma ordem de serviço
    /// </summary>
    /// <param name="id">ID da ordem de serviço</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação do início do diagnóstico</returns>
    [HttpPut("{id}/IniciarDiagnostico")]
    [Authorize(Roles = "Administrador,Mecanico")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IniciarDiagnostico([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.IniciarDiagnostico(id, idUsuario, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Realiza o diagnóstico de uma ordem de serviço
    /// </summary>
    /// <param name="id">ID da ordem de serviço</param>
    /// <param name="request">Dados do diagnóstico</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação do diagnóstico realizado</returns>
    [HttpPut("{id}/RealizarDiagnostico")]
    [Authorize(Roles = "Administrador,Mecanico")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RealizarDiagnostico([FromRoute] Guid id, [FromBody] DiagnosticoRequestDTO request, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.RealizarDiagnostico(id, idUsuario, request, ct);
        return response.ToResult();
    }

    /// <summary>
    /// Registra a entrega de uma ordem de serviço
    /// </summary>
    /// <param name="id">ID da ordem de serviço</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação do registro de entrega</returns>
    [HttpPut("{id}/RegistrarEntrega")]
    [Authorize(Roles = "Administrador,Funcionario")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrarEntrega([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _ordemService.RegistrarEntrega(id, idUsuario, ct);
        return response.ToResult();
    }
}

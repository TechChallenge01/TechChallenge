using API.Extensions;
using Application.Servicos.DTOs.Requests;
using Application.Servicos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers;

/// <summary>
/// Controller responsável por gerenciar serviços
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ServicoController : ControllerBase
{
    private readonly IServicoService _servicoService;
    public ServicoController(IServicoService servicoService)
    {
        _servicoService = servicoService;
    }

    /// <summary>
    /// Obtém lista paginada de serviços
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de serviços</returns>
    [HttpGet]
    [Authorize(Roles = "Administrador,Funcionario,Mecanico")]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var response = await _servicoService.GetPaginated(page, pageSize, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Cria um novo serviço
    /// </summary>
    /// <param name="request">Dados do serviço a ser criado</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>ID do serviço criado</returns>
    [HttpPost]
    [Authorize(Roles = "Administrador,Funcionario")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ServicoRequestDTO request, CancellationToken ct) 
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _servicoService.Create(request, idUsuario, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Deleta um serviço
    /// </summary>
    /// <param name="id">ID do serviço a ser deletado</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador,Funcionario")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _servicoService.Delete(id, idUsuario, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Atualiza um serviço existente
    /// </summary>
    /// <param name="id">ID do serviço a ser atualizado</param>
    /// <param name="request">Dados atualizados do serviço</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação de atualização</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Funcionario")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ServicoRequestDTO request, CancellationToken ct) 
    {
        var idUsuario = User.ObterIdUsuario();

        var response = await _servicoService.Update(id, idUsuario, request, ct);

        return response.ToResult();
    }

    /// <summary>
    /// Obtém um serviço por ID
    /// </summary>
    /// <param name="id">ID do serviço</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do serviço</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Funcionario,Mecanico")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var response = await _servicoService.GetById(id, ct);

        return response.ToResult();
    }
}

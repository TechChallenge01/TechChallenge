using API.Extensions;
using Application.Clientes.DTOs.Requests;
using Application.Clientes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar clientes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Obtém lista paginada de clientes
        /// </summary>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Lista paginada de clientes</returns>
        [HttpGet]
        [Authorize(Roles = "Administrador,Funcionario")]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var response = await _clienteService.GetPaginated(page, pageSize, ct);

            return response.ToResult();
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="request">Dados do cliente a ser criado</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>ID do cliente criado</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador,Funcionario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] ClienteRequestDTO request, CancellationToken ct)
        {
            var idUsuario = User.ObterIdUsuario();

            var response = await _clienteService.Create(request, idUsuario, ct);

            return response.ToResult();
        }

        /// <summary>
        /// Deleta um cliente
        /// </summary>
        /// <param name="id">ID do cliente a ser deletado</param>
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

            var response = await _clienteService.Delete(id, idUsuario, ct);

            return response.ToResult();
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="request">Dados atualizados do cliente</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Confirmação de atualização</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Funcionario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ClienteRequestDTO request, CancellationToken ct)
        {
            var idUsuario = User.ObterIdUsuario();

            var response = await _clienteService.Update(id, idUsuario, request, ct);

            return response.ToResult();
        }

        /// <summary>
        /// Obtém um cliente por ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Dados do cliente</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Funcionario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var response = await _clienteService.GetById(id, ct);

            return response.ToResult();
        }
    }
}

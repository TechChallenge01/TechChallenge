using API.Extensions;
using Application.Pecas.DTOs.Requests;
using Application.Pecas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar peças
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PecaController : ControllerBase
    {
        private readonly IPecaService _pecaService;
        public PecaController(IPecaService pecaService)
        {
            _pecaService = pecaService;
        }

        /// <summary>
        /// Obtém lista paginada de peças
        /// </summary>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Lista paginada de peças</returns>
        [HttpGet]
        [Authorize(Roles = "Administrador,Funcionario,Mecanico,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _pecaService.GetPaginated(page, pageSize, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Cria uma nova peça
        /// </summary>
        /// <param name="request">Dados da peça a ser criada</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>ID da peça criada</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] PecaRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _pecaService.Create(request, idUsuario, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Deleta uma peça
        /// </summary>
        /// <param name="id">ID da peça a ser deletada</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Confirmação de exclusão</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _pecaService.Delete(id, idUsuario, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Atualiza uma peça existente
        /// </summary>
        /// <param name="id">ID da peça a ser atualizada</param>
        /// <param name="request">Dados atualizados da peça</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Confirmação de atualização</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PecaRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _pecaService.Update(id, idUsuario, request, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Obtém uma peça por ID
        /// </summary>
        /// <param name="id">ID da peça</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Dados da peça</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Funcionario,Mecanico,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _pecaService.GetById(id, ct);

            return result.ToResult();
        }
    }
}

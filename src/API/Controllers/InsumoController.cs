using API.Extensions;
using Application.Insumos.DTOs.Requests;
using Application.Insumos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar insumos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InsumoController : ControllerBase
    {
        private readonly IInsumoService _insumoService;
        public InsumoController(IInsumoService insumoService)
        {
            _insumoService = insumoService;
        }

        /// <summary>
        /// Obtém lista paginada de insumos
        /// </summary>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Lista paginada de insumos</returns>
        [HttpGet]
        [Authorize(Roles = "Administrador,Funcionario,Mecanico,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _insumoService.GetPaginated(page, pageSize, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Obtém um insumo por ID
        /// </summary>
        /// <param name="id">ID do insumo</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Dados do insumo</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Funcionario,Mecanico,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _insumoService.GetById(id, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Cria um novo insumo
        /// </summary>
        /// <param name="request">Dados do insumo a ser criado</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>ID do insumo criado</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] InsumoRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _insumoService.Create(request, idUsuario, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Atualiza um insumo existente
        /// </summary>
        /// <param name="id">ID do insumo a ser atualizado</param>
        /// <param name="request">Dados atualizados do insumo</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Confirmação de atualização</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] InsumoRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _insumoService.Update(id, idUsuario, request, ct);

            return result.ToResult();
        }

        /// <summary>
        /// Deleta um insumo
        /// </summary>
        /// <param name="id">ID do insumo a ser deletado</param>
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

            var result = await _insumoService.Delete(id, idUsuario, ct);

            return result.ToResult();
        }
    }
}

using API.Extensions;
using Application.Estoques.DTOs.Requests;
using Application.Estoques.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar estoque
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _estoqueService;
        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        /// <summary>
        /// Obtém lista paginada de movimentações de estoque
        /// </summary>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Lista paginada de movimentações de estoque</returns>
        [HttpGet]
        [Authorize(Roles = "Administrador,Funcionario,Mecanico,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct= default)
        {
            var result = await _estoqueService.GetPaginated(page, pageSize, ct); 

            return result.ToResult();
        }

        /// <summary>
        /// Obtém uma movimentação de estoque por ID
        /// </summary>
        /// <param name="Id">ID da movimentação de estoque</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Dados da movimentação de estoque</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Funcionario,Mecanico,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] Guid Id, CancellationToken ct= default)
        {
            var result = await _estoqueService.GetById(Id, ct); 

            return result.ToResult();
        }

        /// <summary>
        /// Realiza uma movimentação de estoque
        /// </summary>
        /// <param name="request">Dados da movimentação de estoque</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Confirmação da movimentação</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador,Funcionario,Almoxarifado")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Movimentar([FromBody] EstoqueRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _estoqueService.Movimetar(request, idUsuario, ct);

            return result.ToResult();
        }
    }
}

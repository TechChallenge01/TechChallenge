using API.Extensions;
using Application.Auth.DTOs.Requests;
using Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IAuthService _authService;
        public UsuarioController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="request">Dados do usuário a ser criado</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Confirmação de criação do usuário</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioRequestDTO request, CancellationToken ct)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _authService.CriarUsuario(request, idUsuario, ct);

            return result.ToResult();
        }
    }
}

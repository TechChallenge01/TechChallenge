using Application.Auth.DTOs.Requests;
using Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IAuthService _authService;
        public UsuarioController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioRequestDTO request, CancellationToken ct)
        {
            var result = await _authService.CriarUsuario(request, ct);

            return result.ToResult();
        }
    }
}

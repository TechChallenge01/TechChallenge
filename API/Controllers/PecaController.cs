using API.Extensions;
using Application.Pecas.DTOs.Requests;
using Application.Pecas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PecaController : ControllerBase
    {
        private readonly IPecaService _pecaService;
        public PecaController(IPecaService pecaService)
        {
            _pecaService = pecaService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario,Mecanico,Almoxarifado")]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _pecaService.GetPaginated(page, pageSize, ct);

            return result.ToResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Almoxarifado")]
        public async Task<IActionResult> Create([FromBody] PecaRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _pecaService.Create(request, idUsuario, ct);

            return result.ToResult();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Almoxarifado")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _pecaService.Delete(id, idUsuario, ct);

            return result.ToResult();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Almoxarifado")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PecaRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _pecaService.Update(id, idUsuario, request, ct);

            return result.ToResult();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Funcionario,Mecanico,Almoxarifado")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _pecaService.GetById(id, ct);

            return result.ToResult();
        }
    }
}

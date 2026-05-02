using API.Extensions;
using Application.Veiculos.DTOs.Requests;
using Application.Veiculos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoService _veiculoService;

        public VeiculoController(IVeiculoService veiculoService)
        {
            _veiculoService = veiculoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _veiculoService.GetPaginated(page, pageSize, ct);

            return result.ToResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Create([FromBody] VeiculoRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _veiculoService.Create(request, idUsuario, ct);

            return result.ToResult();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _veiculoService.Delete(id, idUsuario, ct);

            return result.ToResult();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update([FromRoute] Guid Id,[FromBody] VeiculoRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _veiculoService.Update(Id, idUsuario, request, ct);

            return result.ToResult();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _veiculoService.GetById(id, ct);

            return result.ToResult();
        }
    }
}

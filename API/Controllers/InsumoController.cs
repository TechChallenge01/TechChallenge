using API.Extensions;
using Application.Insumos.DTOs.Requests;
using Application.Insumos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsumoController : ControllerBase
    {
        private readonly IInsumoService _insumoService;
        public InsumoController(IInsumoService insumoService)
        {
            _insumoService = insumoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario,Mecanico,Almoxarifado")]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _insumoService.GetPaginated(page, pageSize, ct);

            return result.ToResult();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Funcionario,Mecanico,Almoxarifado")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _insumoService.GetById(id, ct);

            return result.ToResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Almoxarifado")]
        public async Task<IActionResult> Create([FromBody] InsumoRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _insumoService.Create(request, idUsuario, ct);

            return result.ToResult();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Almoxarifado")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] InsumoRequestDTO request, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _insumoService.Update(id, idUsuario, request, ct);

            return result.ToResult();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Almoxarifado")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        {
            var idUsuario = User.ObterIdUsuario();

            var result = await _insumoService.Delete(id, idUsuario, ct);
            
            return result.ToResult();
        }
    }
}

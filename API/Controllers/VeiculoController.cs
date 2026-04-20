using Application.Veiculos.DTOs.Requests;
using Application.Veiculos.Services;
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
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _veiculoService.GetPaginated(page, pageSize, ct);

            return result.ToResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeiculoRequestDTO request, CancellationToken ct = default)
        {
            var result = await _veiculoService.Create(request, ct);

            return result.ToResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _veiculoService.Delete(id, ct);

            return result.ToResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid Id,[FromBody] VeiculoRequestDTO request, CancellationToken ct = default)
        {
            var result = await _veiculoService.Update(Id, request, ct);

            return result.ToResult();
        }
    }
}

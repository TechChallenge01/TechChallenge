using Application.Insumos.DTOs.Requests;
using Application.Insumos.Services;
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
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _insumoService.GetPaginated(page, pageSize, ct);

            return result.ToResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _insumoService.GetById(id, ct);

            return result.ToResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InsumoRequestDTO request, CancellationToken ct = default)
        {
            var result = await _insumoService.Create(request, ct);

            return result.ToResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] InsumoRequestDTO request, CancellationToken ct = default)
        {
            var result = await _insumoService.Update(id, request, ct);

            return result.ToResult();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _insumoService.Delete(id, ct);
            
            return result.ToResult();
        }
    }
}

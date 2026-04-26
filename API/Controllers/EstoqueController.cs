using Application.Estoques.DTOs.Requests;
using Application.Estoques.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _estoqueService;
        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct= default)
        {
            var result = await _estoqueService.GetPaginated(page, pageSize, ct); 

            return result.ToResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid Id, CancellationToken ct= default)
        {
            var result = await _estoqueService.GetById(Id, ct); 

            return result.ToResult();
        }

        [HttpPost]
        public async Task<IActionResult> Movimentar([FromBody] EstoqueRequestDTO request, CancellationToken ct = default)
        {
            var result = await _estoqueService.Movimetar(request, ct);

            return result.ToResult();
        }
    }
}

using Application.Clientes.DTOs.Requests;
using Application.Clientes.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var response = await _clienteService.GetPaginated(page, pageSize, ct);

            return response.ToResult();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteRequestDTO request, CancellationToken ct)
        {
            var response = await _clienteService.Create(request, ct);

            return response.ToResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var response = await _clienteService.Delete(id, ct);

            return response.ToResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ClienteRequestDTO request, CancellationToken ct)
        {
            var response = await _clienteService.Update(id, request, ct);

            return response.ToResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var response = await _clienteService.GetById(id, ct);

            return response.ToResult();
        }
    }
}

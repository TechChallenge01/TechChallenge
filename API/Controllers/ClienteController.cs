using Application.Cliente.DTOs.Requests;
using Application.Cliente.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

namespace API.Controllers
{
    [ApiController]
    [Route("Cliente")]
    public class ClienteController(IClienteService _clienteService) : ControllerBase
    {
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

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var response = await _clienteService.Delete(id, ct);

            return response.ToResult();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClienteRequestDTO request, CancellationToken ct)
        {
            var response = await _clienteService.Update(id, request, ct);

            return response.ToResult();
        }
    }
}

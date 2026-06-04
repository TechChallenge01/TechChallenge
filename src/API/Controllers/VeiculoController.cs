using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar veículos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculoController : ControllerBase
    {
        //private readonly IVeiculoService _veiculoService;

        //public VeiculoController(IVeiculoService veiculoService)
        //{
        //    _veiculoService = veiculoService;
        //}

        ///// <summary>
        ///// Obtém lista paginada de veículos
        ///// </summary>
        ///// <param name="page">Número da página (padrão: 1)</param>
        ///// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
        ///// <param name="ct">Token de cancelamento</param>
        ///// <returns>Lista paginada de veículos</returns>
        //[HttpGet]
        //[Authorize(Roles = "Administrador,Funcionario")]
        //[ProducesResponseType(StatusCodes.Status206PartialContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        //{
        //    var result = await _veiculoService.GetPaginated(page, pageSize, ct);

        //    return result.ToResult();
        //}

        ///// <summary>
        ///// Cria um novo veículo
        ///// </summary>
        ///// <param name="request">Dados do veículo a ser criado</param>
        ///// <param name="ct">Token de cancelamento</param>
        ///// <returns>ID do veículo criado</returns>
        //[HttpPost]
        //[Authorize(Roles = "Administrador,Funcionario")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Create([FromBody] VeiculoRequestDTO request, CancellationToken ct = default)
        //{
        //    var idUsuario = User.ObterIdUsuario();

        //    var result = await _veiculoService.Create(request, idUsuario, ct);

        //    return result.ToResult();
        //}

        ///// <summary>
        ///// Deleta um veículo
        ///// </summary>
        ///// <param name="id">ID do veículo a ser deletado</param>
        ///// <param name="ct">Token de cancelamento</param>
        ///// <returns>Confirmação de exclusão</returns>
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Administrador,Funcionario")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct = default)
        //{
        //    var idUsuario = User.ObterIdUsuario();

        //    var result = await _veiculoService.Delete(id, idUsuario, ct);

        //    return result.ToResult();
        //}

        ///// <summary>
        ///// Atualiza um veículo existente
        ///// </summary>
        ///// <param name="Id">ID do veículo a ser atualizado</param>
        ///// <param name="request">Dados atualizados do veículo</param>
        ///// <param name="ct">Token de cancelamento</param>
        ///// <returns>Confirmação de atualização</returns>
        //[HttpPut("{id}")]
        //[Authorize(Roles = "Administrador,Funcionario")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Update([FromRoute] Guid Id,[FromBody] VeiculoRequestDTO request, CancellationToken ct = default)
        //{
        //    var idUsuario = User.ObterIdUsuario();

        //    var result = await _veiculoService.Update(Id, idUsuario, request, ct);

        //    return result.ToResult();
        //}

        ///// <summary>
        ///// Obtém um veículo por ID
        ///// </summary>
        ///// <param name="id">ID do veículo</param>
        ///// <param name="ct">Token de cancelamento</param>
        ///// <returns>Dados do veículo</returns>
        //[HttpGet("{id}")]
        //[Authorize(Roles = "Administrador,Funcionario")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
        //{
        //    var result = await _veiculoService.GetById(id, ct);

        //    return result.ToResult();
        //}
    }
}

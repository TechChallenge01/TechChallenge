using Application.Auth.DTOs.Requests;
using Application.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken ct)
    {
        var result = await _authService.Login(request, ct);

        return result.StatusCode switch 
        {
            HttpStatusCode.OK => Ok(result), 
            HttpStatusCode.Unauthorized => Unauthorized(result),_ => StatusCode(500, result)
        };
    }

    [HttpPost("usuarios")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioRequestDTO request, CancellationToken ct)
    {
        var result = await _authService.CriarUsuario(request, ct);

        return result.StatusCode switch
        {
            HttpStatusCode.Created => Created(string.Empty, result),
            HttpStatusCode.BadRequest => BadRequest(result),
            _ => StatusCode(500, result)
        };
    }
}

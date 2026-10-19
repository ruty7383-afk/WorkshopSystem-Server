using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopSystem.Application.DTOs.Auth;
using WorkshopSystem.Application.Interfaces.Services;

namespace WorkshopSystem.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterRequestDto dto,
        CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);

        return Ok(result);
    }
    
}
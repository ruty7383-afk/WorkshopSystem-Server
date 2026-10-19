using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopSystem.Application.DTOs.Registrations;
using WorkshopSystem.Application.Interfaces.Services;

namespace WorkshopSystem.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public UsersController(
        IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpGet("me/registrations")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<
        IReadOnlyList<RegistrationResponseDto>>>
        GetMyRegistrations(CancellationToken ct)
    {
        var userId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _registrationService
                .GetMyRegistrationsAsync(userId, ct);

        return Ok(result);
    }
}
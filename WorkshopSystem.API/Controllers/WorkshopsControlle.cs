using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkshopSystem.Application.DTOs.Common;
using WorkshopSystem.Application.DTOs.Registrations;
using WorkshopSystem.Application.DTOs.Workshops;
using WorkshopSystem.Application.Interfaces.Services;
namespace WorkshopSystem.API.Controllers;

[ApiController]
[Route("api/workshops")]
public class WorkshopsController : ControllerBase
{
    private readonly IWorkshopService _workshopService;
    private readonly IRegistrationService _registrationService;
    public WorkshopsController(
    IWorkshopService workshopService,
    IRegistrationService registrationService)
    {
        _workshopService = workshopService;
        _registrationService = registrationService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<WorkshopResponseDto>>> GetAll(
        [FromQuery] WorkshopQueryDto query,
        CancellationToken ct)
    {
        var result =
            await _workshopService.GetPagedAsync(query, ct);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<WorkshopDetailsResponseDto>> GetById(
        [FromRoute] int id,
        CancellationToken ct)
    {
        var result =
            await _workshopService.GetByIdAsync(id, ct);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<WorkshopDetailsResponseDto>> Create(
        [FromBody] CreateWorkshopRequestDto request,
        CancellationToken ct)
    {
        var result =
            await _workshopService.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateWorkshopRequestDto request,
        CancellationToken ct)
    {
        await _workshopService.UpdateAsync(id, request, ct);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id,
        CancellationToken ct)
    {
        await _workshopService.DeleteAsync(id, ct);

        return NoContent();
    }
    [HttpPost("{id:int}/registrations")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<RegistrationResponseDto>> Register(
    int id,
    CancellationToken ct)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result =
            await _registrationService.RegisterAsync(
                id, userId, ct);

        return Created(
            "/api/users/me/registrations",
            result);
    }

    [HttpDelete("{id:int}/registrations/me")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CancelRegistration(
        int id,
        CancellationToken ct)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _registrationService.CancelAsync(
            id, userId, ct);

        return NoContent();
    }

    [HttpGet("{id:int}/registrations")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<
        IReadOnlyList<WorkshopRegistrantResponseDto>>>
        GetRegistrations(
            int id,
            CancellationToken ct)
    {
        var result =
            await _registrationService
                .GetWorkshopRegistrationsAsync(id, ct);

        return Ok(result);
    }
}
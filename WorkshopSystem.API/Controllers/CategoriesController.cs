using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopSystem.Application.DTOs.Categories;
using WorkshopSystem.Application.Interfaces.Services;

namespace WorkshopSystem.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetAll(
        CancellationToken ct)
    {
        var result = await _categoryService.GetAllAsync(ct);
        return Ok(result);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponseDto>> Create(
    [FromBody] CreateCategoryRequestDto dto,
    CancellationToken ct)
    {
        var result = await _categoryService.CreateAsync(dto, ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
    int id,
    [FromBody] UpdateCategoryRequestDto dto,
    CancellationToken ct)
    {
        await _categoryService.UpdateAsync(id, dto, ct);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        await _categoryService.DeleteAsync(id, ct);
        return NoContent();
    }
}
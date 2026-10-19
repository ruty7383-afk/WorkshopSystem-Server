using AutoMapper;
using WorkshopSystem.Application.DTOs.Categories;
using WorkshopSystem.Application.Exceptions;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Application.Interfaces.Services;
using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(
        CancellationToken ct)
    {
        var categories = await _categoryRepository.GetAllAsync(ct);

        return _mapper.Map<IReadOnlyList<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto> CreateAsync(
        CreateCategoryRequestDto dto,
        CancellationToken ct)
    {
        if (await _categoryRepository.ExistsByNameAsync(dto.Name, null, ct))
            throw new ConflictException("Category name already exists.");

        var category = _mapper.Map<Category>(dto);

        await _categoryRepository.AddAsync(category, ct);
        await _categoryRepository.SaveChangesAsync(ct);

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task UpdateAsync(
        int id,
        UpdateCategoryRequestDto dto,
        CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);

        if (category == null)
            throw new NotFoundException("Category not found.");

        if (await _categoryRepository.ExistsByNameAsync(dto.Name, id, ct))
            throw new ConflictException("Category name already exists.");

        _mapper.Map(dto, category);

        await _categoryRepository.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);

        if (category == null)
            throw new NotFoundException("Category not found.");

        if (await _categoryRepository.IsInUseAsync(id, ct))
            throw new BusinessException(
                "Cannot delete a category that is used by workshops.");

        _categoryRepository.Remove(category);

        await _categoryRepository.SaveChangesAsync(ct);
    }
}
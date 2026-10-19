using AutoMapper;
using WorkshopSystem.Application.DTOs.Categories;
using WorkshopSystem.Application.DTOs.Registrations;
using WorkshopSystem.Application.DTOs.Workshops;
using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category
        CreateMap<CreateCategoryRequestDto, Category>();
        CreateMap<UpdateCategoryRequestDto, Category>();
        CreateMap<Category, CategoryResponseDto>();

        // Workshop - Request -> Entity
        CreateMap<CreateWorkshopRequestDto, Workshop>();
        CreateMap<UpdateWorkshopRequestDto, Workshop>();

        // Workshop - Entity -> Response
        CreateMap<Workshop, WorkshopResponseDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ActiveRegistrationsCount,
                opt => opt.Ignore())
            .ForMember(dest => dest.AvailablePlaces,
                opt => opt.Ignore())
            .ForMember(dest => dest.Status,
                opt => opt.Ignore());

        CreateMap<Workshop, WorkshopDetailsResponseDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ActiveRegistrationsCount,
                opt => opt.Ignore())
            .ForMember(dest => dest.AvailablePlaces,
                opt => opt.Ignore())
            .ForMember(dest => dest.Status,
                opt => opt.Ignore());
        CreateMap<Registration, RegistrationResponseDto>()
    .ForMember(dest => dest.WorkshopName,
        opt => opt.MapFrom(src => src.Workshop.Name))
    .ForMember(dest => dest.StartDate,
        opt => opt.MapFrom(src => src.Workshop.StartDate))
    .ForMember(dest => dest.StartTime,
        opt => opt.MapFrom(src => src.Workshop.StartTime));

        CreateMap<Registration, WorkshopRegistrantResponseDto>()
            .ForMember(dest => dest.RegistrationId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.User.Email));
    }
}
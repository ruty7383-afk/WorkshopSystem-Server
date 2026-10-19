using AutoMapper;
using Moq;
using WorkshopSystem.Application.DTOs.Workshops;
using WorkshopSystem.Application.Exceptions;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Application.Services;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Domain.Enums;

namespace WorkshopSystem.Tests.Services;

public class WorkshopServiceTests
{
    [Fact]
    public async Task UpdateAsync_WhenCapacityBelowActiveRegistrations_ThrowsConflictException()
    {
        // Arrange
        var workshopRepository =
            new Mock<IWorkshopRepository>();

        var categoryRepository =
            new Mock<ICategoryRepository>();

        var mapper =
            new Mock<IMapper>();

        var workshop = new Workshop
        {
            Id = 1,
            Name = "C# Workshop",

            StartDate = DateTime.SpecifyKind(
                new DateTime(2026, 11, 1),
                DateTimeKind.Utc),

            DayOfWeek = DayOfWeek.Sunday,
            StartTime = new TimeOnly(18, 0),
            DurationMinutes = 90,
            NumberOfMeetings = 4,
            Price = 250,
            Capacity = 10,
            CategoryId = 1,

            Registrations = new List<Registration>
            {
                new Registration
                {
                    Id = 1,
                    UserId = 10,
                    WorkshopId = 1,
                    Status = RegistrationStatus.Active
                },

                new Registration
                {
                    Id = 2,
                    UserId = 11,
                    WorkshopId = 1,
                    Status = RegistrationStatus.Active
                }
            }
        };

        var request = new UpdateWorkshopRequestDto
        {
            Name = "C# Workshop",
            Description = "Updated workshop",

            StartDate = DateTime.SpecifyKind(
                new DateTime(2026, 11, 1),
                DateTimeKind.Utc),

            DayOfWeek = DayOfWeek.Sunday,
            StartTime = new TimeOnly(18, 0),
            DurationMinutes = 90,
            NumberOfMeetings = 4,
            Price = 250,

            // יש 2 נרשמים פעילים
            Capacity = 1,

            CategoryId = 1
        };

        workshopRepository
            .Setup(r => r.GetByIdWithRegistrationsAsync(
                workshop.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workshop);

        categoryRepository
            .Setup(r => r.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category
            {
                Id = 1,
                Name = "Programming"
            });

        var service = new WorkshopService(
            workshopRepository.Object,
            categoryRepository.Object,
            mapper.Object);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            service.UpdateAsync(
                workshop.Id,
                request,
                CancellationToken.None));

        workshopRepository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
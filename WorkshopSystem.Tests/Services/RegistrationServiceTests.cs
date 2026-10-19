using AutoMapper;
using Moq;
using WorkshopSystem.Application.Exceptions;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Application.Services;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Domain.Enums;

namespace WorkshopSystem.Tests.Services;

public class RegistrationServiceTests
{
    private readonly Mock<IWorkshopRepository> _workshopRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RegistrationService _service;

    public RegistrationServiceTests()
    {
        _workshopRepositoryMock = new Mock<IWorkshopRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new RegistrationService(
            _workshopRepositoryMock.Object,
            _mapperMock.Object);
    }

    private Workshop CreateFutureWorkshop(
        int capacity = 10,
        int workshopId = 1)
    {
        return new Workshop
        {
            Id = workshopId,
            Name = "C# Workshop",

            StartDate = DateTime.SpecifyKind(
                DateTime.UtcNow.Date.AddDays(7),
                DateTimeKind.Utc),

            DayOfWeek =
                DateTime.UtcNow.Date.AddDays(7).DayOfWeek,

            StartTime = new TimeOnly(18, 0),

            DurationMinutes = 90,
            NumberOfMeetings = 4,
            Price = 250,
            Capacity = capacity,
            CategoryId = 1,
            Version = Guid.NewGuid(),

            Registrations = new List<Registration>()
        };
    }

    [Fact]
    public async Task RegisterAsync_Success_AddsRegistration()
    {
        // Arrange
        var workshop = CreateFutureWorkshop();

        _workshopRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(
                workshop.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workshop);

        _workshopRepositoryMock
            .Setup(r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.RegisterAsync(
            workshop.Id,
            5,
            CancellationToken.None);

        // Assert
        Assert.Single(workshop.Registrations);

        var registration = workshop.Registrations.First();

        Assert.Equal(5, registration.UserId);
        Assert.Equal(workshop.Id, registration.WorkshopId);
        Assert.Equal(RegistrationStatus.Active, registration.Status);
        Assert.Equal(workshop.Price, registration.AmountPaid);
        Assert.Equal(0, registration.RefundedAmount);

        _workshopRepositoryMock.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenWorkshopFull_ThrowsConflictException()
    {
        // Arrange
        var workshop = CreateFutureWorkshop(capacity: 1);

        workshop.Registrations.Add(new Registration
        {
            Id = 1,
            UserId = 20,
            WorkshopId = workshop.Id,
            Status = RegistrationStatus.Active,
            AmountPaid = workshop.Price,
            RefundedAmount = 0
        });

        _workshopRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(
                workshop.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workshop);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.RegisterAsync(
                workshop.Id,
                5,
                CancellationToken.None));

        _workshopRepositoryMock.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenAlreadyActive_ThrowsConflictException()
    {
        // Arrange
        const int userId = 5;

        var workshop = CreateFutureWorkshop();

        workshop.Registrations.Add(new Registration
        {
            Id = 1,
            UserId = userId,
            WorkshopId = workshop.Id,
            Status = RegistrationStatus.Active,
            AmountPaid = workshop.Price,
            RefundedAmount = 0
        });

        _workshopRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(
                workshop.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workshop);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.RegisterAsync(
                workshop.Id,
                userId,
                CancellationToken.None));

        _workshopRepositoryMock.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenWorkshopStarted_ThrowsConflictException()
    {
        // Arrange
        var workshop = CreateFutureWorkshop();

        workshop.StartDate = DateTime.SpecifyKind(
            DateTime.UtcNow.Date.AddDays(-7),
            DateTimeKind.Utc);

        workshop.DayOfWeek = workshop.StartDate.DayOfWeek;

        _workshopRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(
                workshop.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workshop);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.RegisterAsync(
                workshop.Id,
                5,
                CancellationToken.None));

        _workshopRepositoryMock.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CancelAsync_Success_CancelsAndRefundsRegistration()
    {
        // Arrange
        const int userId = 5;

        var workshop = CreateFutureWorkshop();

        var registration = new Registration
        {
            Id = 1,
            UserId = userId,
            WorkshopId = workshop.Id,
            Workshop = workshop,
            Status = RegistrationStatus.Active,
            AmountPaid = 250,
            RefundedAmount = 0
        };

        workshop.Registrations.Add(registration);

        _workshopRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(
                workshop.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workshop);

        _workshopRepositoryMock
            .Setup(r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.CancelAsync(
            workshop.Id,
            userId,
            CancellationToken.None);

        // Assert
        Assert.Equal(
            RegistrationStatus.Cancelled,
            registration.Status);

        Assert.NotNull(registration.CancelledAtUtc);

        Assert.Equal(
            registration.AmountPaid,
            registration.RefundedAmount);

        _workshopRepositoryMock.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WhenWorkshopStarted_ThrowsConflictException()
    {
        // Arrange
        const int userId = 5;

        var workshop = CreateFutureWorkshop();

        workshop.StartDate = DateTime.SpecifyKind(
            DateTime.UtcNow.Date.AddDays(-7),
            DateTimeKind.Utc);

        workshop.DayOfWeek = workshop.StartDate.DayOfWeek;

        workshop.Registrations.Add(new Registration
        {
            Id = 1,
            UserId = userId,
            WorkshopId = workshop.Id,
            Status = RegistrationStatus.Active,
            AmountPaid = 250,
            RefundedAmount = 0
        });

        _workshopRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(
                workshop.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workshop);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.CancelAsync(
                workshop.Id,
                userId,
                CancellationToken.None));

        _workshopRepositoryMock.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
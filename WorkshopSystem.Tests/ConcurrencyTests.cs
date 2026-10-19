using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Infrastructure.Data;

namespace WorkshopSystem.Tests;

public class ConcurrencyTests
{
    [Fact]
    public async Task TwoContexts_UpdatingSameWorkshop_SecondSaveThrowsConcurrencyException()
    {
        var connectionString =
            Environment.GetEnvironmentVariable("WORKSHOP_TEST_DB");

        Assert.False(
            string.IsNullOrWhiteSpace(connectionString),
            "WORKSHOP_TEST_DB environment variable is missing.");

        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

        int workshopId;
        int categoryId;

        // יצירת נתוני בדיקה
        await using (var setupContext = new AppDbContext(options))
        {
            var category = new Category
            {
                Name = $"Concurrency Test {Guid.NewGuid():N}"
            };

            setupContext.Categories.Add(category);
            await setupContext.SaveChangesAsync();

            var startDate = DateTime.SpecifyKind(
                DateTime.UtcNow.Date.AddDays(30),
                DateTimeKind.Utc);

            var workshop = new Workshop
            {
                Name = $"Concurrency Workshop {Guid.NewGuid():N}",
                Description = "Concurrency integration test",
                StartDate = startDate,
                DayOfWeek = startDate.DayOfWeek,
                StartTime = new TimeOnly(18, 0),
                DurationMinutes = 90,
                NumberOfMeetings = 4,
                Price = 200,
                Capacity = 10,
                CategoryId = category.Id,
                Version = Guid.NewGuid()
            };

            setupContext.Workshops.Add(workshop);
            await setupContext.SaveChangesAsync();

            workshopId = workshop.Id;
            categoryId = category.Id;
        }

        try
        {
            await using var contextA =
                new AppDbContext(options);

            await using var contextB =
                new AppDbContext(options);

            // שני Contexts קוראים אותה גרסה
            var workshopA =
                await contextA.Workshops
                    .SingleAsync(w => w.Id == workshopId);

            var workshopB =
                await contextB.Workshops
                    .SingleAsync(w => w.Id == workshopId);

            var originalVersionA = workshopA.Version;
            var originalVersionB = workshopB.Version;

            Assert.Equal(
                originalVersionA,
                originalVersionB);

            // Context A משנה ושומר
            workshopA.Capacity = 11;

            await contextA.SaveChangesAsync();

            // Context B עדיין מחזיק את ה-Version הישן
            workshopB.Price = 250;

            // ולכן השמירה השנייה חייבת להיכשל
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () =>
                {
                    await contextB.SaveChangesAsync();
                });
        }
        finally
        {
            // ניקוי נתוני הבדיקה
            await using var cleanupContext =
                new AppDbContext(options);

            var workshop =
                await cleanupContext.Workshops
                    .FirstOrDefaultAsync(w => w.Id == workshopId);

            if (workshop != null)
            {
                cleanupContext.Workshops.Remove(workshop);
                await cleanupContext.SaveChangesAsync();
            }

            var category =
                await cleanupContext.Categories
                    .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category != null)
            {
                cleanupContext.Categories.Remove(category);
                await cleanupContext.SaveChangesAsync();
            }
        }
    }
}
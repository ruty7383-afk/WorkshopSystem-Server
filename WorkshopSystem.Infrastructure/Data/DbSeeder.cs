using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Domain.Enums;

namespace WorkshopSystem.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(

        AppDbContext context,
        PasswordHasher<User> passwordHasher)
    {
        // --------------------
        // Categories
        // --------------------

        var categoryNames = new[]
        {
            "Programming",
            "Databases",
            "AI",
            "Other"
        };

        foreach (var name in categoryNames)
        {
            if (!await context.Categories.AnyAsync(c => c.Name == name))
            {
                context.Categories.Add(new Category
                {
                    Name = name
                });
            }
        }

        await context.SaveChangesAsync();
        // --------------------
        // Users
        // --------------------

        if (!await context.Users.AnyAsync(
                u => u.Email == "admin@workshops.local"))
        {
            var admin = new User
            {
                Name = "Demo Admin",
                Email = "admin@workshops.local",
                Role = UserRole.Admin
            };

            admin.PasswordHash =
                passwordHasher.HashPassword(
                    admin,
                    "Admin123!");

            context.Users.Add(admin);
        }

        if (!await context.Users.AnyAsync(
                u => u.Email == "user@workshops.local"))
        {
            var user = new User
            {
                Name = "Demo User",
                Email = "user@workshops.local",
                Role = UserRole.User
            };

            user.PasswordHash =
                passwordHasher.HashPassword(
                    user,
                    "User123!");

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();

        // --------------------
        // Workshops
        // --------------------

        if (!await context.Workshops.AnyAsync())
        {
            var programming =
                await context.Categories
                    .FirstAsync(c => c.Name == "Programming");

            var databases =
                await context.Categories
                    .FirstAsync(c => c.Name == "Databases");

            var ai =
                await context.Categories
                    .FirstAsync(c => c.Name == "AI");

            var other =
                await context.Categories
                    .FirstAsync(c => c.Name == "Other");

            var date1 = DateTime.SpecifyKind(
                new DateTime(2026, 11, 1),
                DateTimeKind.Utc);

            var date2 = DateTime.SpecifyKind(
                new DateTime(2026, 11, 3),
                DateTimeKind.Utc);

            var date3 = DateTime.SpecifyKind(
                new DateTime(2026, 11, 5),
                DateTimeKind.Utc);

            var date4 = DateTime.SpecifyKind(
                new DateTime(2026, 11, 9),
                DateTimeKind.Utc);

            context.Workshops.AddRange(
                new Workshop
                {
                    Name = "C# Basics",
                    Description = "Introduction to C# programming",
                    StartDate = date1,
                    DayOfWeek = date1.DayOfWeek,
                    StartTime = new TimeOnly(18, 0),
                    DurationMinutes = 90,
                    NumberOfMeetings = 4,
                    Price = 250,
                    Capacity = 10,
                    CategoryId = programming.Id
                },

                new Workshop
                {
                    Name = "PostgreSQL Fundamentals",
                    Description = "Database fundamentals with PostgreSQL",
                    StartDate = date2,
                    DayOfWeek = date2.DayOfWeek,
                    StartTime = new TimeOnly(17, 30),
                    DurationMinutes = 120,
                    NumberOfMeetings = 5,
                    Price = 300,
                    Capacity = 12,
                    CategoryId = databases.Id
                },

                new Workshop
                {
                    Name = "AI Introduction",
                    Description = "Introduction to artificial intelligence",
                    StartDate = date3,
                    DayOfWeek = date3.DayOfWeek,
                    StartTime = new TimeOnly(19, 0),
                    DurationMinutes = 90,
                    NumberOfMeetings = 4,
                    Price = 350,
                    Capacity = 8,
                    CategoryId = ai.Id
                },

                new Workshop
                {
                    Name = "Creative Workshop",
                    Description = "General creative workshop",
                    StartDate = date4,
                    DayOfWeek = date4.DayOfWeek,
                    StartTime = new TimeOnly(18, 30),
                    DurationMinutes = 60,
                    NumberOfMeetings = 3,
                    Price = 180,
                    Capacity = 15,
                    CategoryId = other.Id
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
using SecureFileSender.Api.Models;
using SecureFileSender.Api.Helpers;

namespace SecureFileSender.Api.Data;

public static class Seed
{
	public static void EnsureSeeded(WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		db.Database.EnsureCreated();

		if (!db.AdminUsers.Any())
		{
			db.AdminUsers.Add(new AdminUser
{
            Email = "admin@example.com",
            PasswordHash = PasswordHasher.HashPassword("password123")
        });

			db.SaveChanges();
		}
	}
}

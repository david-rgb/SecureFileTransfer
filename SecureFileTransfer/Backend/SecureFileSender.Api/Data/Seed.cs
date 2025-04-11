using SecureFileSender.Api.Models;
using SecureFileSender.Api.Helpers;

namespace SecureFileSender.Api.Data;

public static class Seed
{
	public static void EnsureSeeded(WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		if (!context.AdminUsers.Any())
		{
			context.AdminUsers.Add(new AdminUser
			{
				Email = "admin@example.com",
				PasswordHash = PasswordHasher.HashPassword("password123")
			});

			context.SaveChanges();
		}
	}
}

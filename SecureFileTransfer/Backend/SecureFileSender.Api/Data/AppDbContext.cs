using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Models;

namespace SecureFileSender.Api.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

	public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
}

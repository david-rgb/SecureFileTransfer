using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Models;

namespace SecureFileSender.Api.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

	public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<SharedFileLink> SharedFileLinks { get; set; }
    public DbSet<Customer> Customers { get; set; }

}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;

namespace SecureFileSender.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("shared-links")]
    public async Task<IActionResult> GetSharedLinks()
    {
        var now = DateTime.UtcNow;

        var links = await _db.SharedFileLinks
            .Include(l => l.Customer)
            .GroupBy(l => l.Token)
            .Select(g => new
            {
                Token = g.Key,
                ReceiverName = g.First().Customer.Name,
                ReceiverEmail = g.First().Customer.Email,
                DownloadCount = g.Count(), // Just a placeholder, replace if you have tracking
                Status = g.First().ExpirationDate < now ? "Expired" : "Pending",
                ExpiresIn = (g.First().ExpirationDate - now).Days,
                SentAt = g.First().ExpirationDate.AddDays(-7) // Assuming 7 day expiry
            })
            .ToListAsync();

        return Ok(links);
    }

    [HttpGet("customers")]
public async Task<IActionResult> GetCustomers()
{
    var customers = await _db.Customers
        .Select(c => new
        {
            Name = c.Name,
            Email = c.Email,
            DownloadCount = _db.SharedFileLinks.Count(l => l.CustomerId == c.Id && l.DownloadCount > 0)
        })
        .ToListAsync();

    return Ok(customers);
}

}

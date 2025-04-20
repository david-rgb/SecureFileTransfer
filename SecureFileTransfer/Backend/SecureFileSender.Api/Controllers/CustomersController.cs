


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using Microsoft.AspNetCore.DataProtection;
namespace SecureFileSender.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProtector _protector;

    public CustomersController(AppDbContext db, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider provider)
{
    _db = db;
    _httpContextAccessor = httpContextAccessor;
    _protector = provider.CreateProtector("UtilityProtector"); // ✅
}

[HttpGet("search")]
public async Task<IActionResult> SearchCustomers([FromQuery] string query)
{
	if (string.IsNullOrWhiteSpace(query)) return Ok(new List<object>());

	var results = await _db.Customers
		.Where(c => c.Email.ToLower().Contains(query.ToLower()))
		.Select(c => new
		{
			c.Email,
			c.Name,
			c.LastName,
			c.Id
		})
		.Take(5)
		.ToListAsync();

	return Ok(results);
}

[Authorize]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteCustomer(int id)
{
	var customer = await _db.Customers.FindAsync(id);
	if (customer == null) return NotFound();
	_db.Customers.Remove(customer);
	await _db.SaveChangesAsync();
	return Ok();
}

[Authorize]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
{
	var customer = await _db.Customers.FindAsync(id);
	if (customer == null) return NotFound();

	customer.Name = dto.Name;
	customer.LastName = dto.LastName;
	await _db.SaveChangesAsync();

	return NoContent();
}



}

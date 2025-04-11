using System;
namespace SecureFileSender.Api.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;

    public List<SharedFileLink> SharedLinks { get; set; } = new();
}

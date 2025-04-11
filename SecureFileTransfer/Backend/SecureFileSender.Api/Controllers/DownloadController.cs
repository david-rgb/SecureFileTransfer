using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using System.Security.Cryptography;
using System.Text;

namespace SecureFileSender.Api.Controllers;

[ApiController]
[Route("download")]
public class RedirectController : ControllerBase
{
    [HttpGet("{slug}")]
    public IActionResult RedirectToFrontend(string slug)
    {
        return Redirect($"http://localhost:5173/download/{slug}");
    }
}

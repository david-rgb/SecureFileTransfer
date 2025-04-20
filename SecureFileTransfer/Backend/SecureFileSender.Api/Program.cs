using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SecureFileSender.Api.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using SecureFileSender.Api.Services;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDataProtection();
builder.Services.AddScoped<EmailService>();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = null; // Allow very large files
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null; // No size limit
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // or 10L * 1024 * 1024 * 1024 for 10 GB
});

// 🔗 Add EF Core with SQLite

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Allow access to HttpContext from services like controllers
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

// 🔐 Add JWT Auth + claim mapping
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
		};

		// 🧠 Map email claim to HttpContext.User.Identity.Name
		options.Events = new JwtBearerEvents
		{
			OnTokenValidated = context =>
			{
				var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
				if (!string.IsNullOrWhiteSpace(email))
				{
					var identity = context.Principal?.Identity as ClaimsIdentity;
					identity?.AddClaim(new Claim(ClaimTypes.Name, email));
				}
				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🌐 CORS for frontend integration
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
	});
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Secure File Sender API",
        Version = "v1",
        Description = "API for sending secure files and emails"
    });

    // Add JWT Auth support
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token **_only_**",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors();
// app.UseHttpsRedirection(); // Uncomment if using HTTPS in production

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Secure File Sender API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 🧠 Apply any pending migrations + seed data
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate(); // Ensures schema is up-to-date
	SecureFileSender.Api.Data.Seed.EnsureSeeded(app); // Seed admin user if needed
}

app.Run();

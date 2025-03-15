using System.Text;
using codewave_root_licence_server_core.Interfaces;
using codewave_root_licence_server_core.Services;
using codewave_root_licence_server_infrastructure.Data;
using codewave_root_licence_server_infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace codewave_roots_licence_server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Setup JWT Authentication
        var key = Encoding.UTF8.GetBytes("YourSuperSecretKeyHere");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        builder.Services.AddAuthorization();


        // Setup Rate Limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed-window", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromMinutes(1); // 1-minute time window
                limiterOptions.PermitLimit = 10; // Allow 10 requests per minute
                limiterOptions.QueueLimit = 2; // Queue up to 2 requests
            });
        });

        // Add DbContext
        builder.Services.AddDbContext<LicenseDbContext>(options =>
            options.UseSqlite("Data Source=licenses.db"));

        // Register Unit of Work & Services
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<LicenseService>();
        
        builder.Services.AddOpenApi();



        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                // Fluent API
                options
                    .WithPreferredScheme("Bearer") // Optional. Security scheme name from the OpenAPI document
                    .WithApiKeyAuthentication(apiKey =>
                    {
                        apiKey.Token = "your-api-key";
                    });

                options
                    .WithHttpBearerAuthentication(bearer =>
                    {
                        bearer.Token = "your-bearer-token";
                    });
            });
        }

        // Generate License
        app.MapPost("/generate", async (LicenseService licenseService, string appName, DateTime expiryDate) =>
        {
            var license = await licenseService.GenerateLicenseAsync(appName, expiryDate);
            return Results.Created($"/licenses/{license.Id}", license);
        }).RequireAuthorization();;

        // Validate License
        app.MapGet("/validate/{key}", async (LicenseService licenseService, string key) =>
        {
            var isValid = await licenseService.ValidateLicenseAsync(key);
            return isValid ? Results.Ok("License is valid.") : Results.BadRequest("Invalid or expired license.");
        }).RequireAuthorization();;

        app.Run();
    }
}
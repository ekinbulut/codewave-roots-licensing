using System.Text;
using codewave_root_licence_server_core.Interfaces;
using codewave_root_licence_server_core.Services;
using codewave_root_licence_server_infrastructure.Data;
using codewave_root_licence_server_infrastructure.UnitOfWork;
using codewave_roots_licence_server.Modules;
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


        // if jwt:key is empty, generate a random key
        if (string.IsNullOrEmpty(builder.Configuration["Jwt:Key"]))
        {
            builder.Configuration["Jwt:Key"] = Guid.NewGuid().ToString();
        }

        // Setup JWT Authentication
        builder.Services.AddAuthenticationModule(builder.Configuration);

        builder.Services.AddAuthorization();

        // Setup Rate Limiting
        builder.Services.AddRateLimitModule();

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
                    .WithApiKeyAuthentication(apiKey => { apiKey.Token = "your-api-key"; });

                options
                    .WithHttpBearerAuthentication(bearer => { bearer.Token = "your-bearer-token"; });
            });
        }

        // Generate License
        app.MapPost("/generate", async (LicenseService licenseService, string appName, DateTime expiryDate) =>
        {
            var license = await licenseService.GenerateLicenseAsync(appName, expiryDate);
            return Results.Created($"/licenses/{license.Id}", license);
        }).RequireAuthorization();
        ;

        // Validate License
        app.MapGet("/validate/{key}", async (LicenseService licenseService, string key) =>
        {
            var isValid = await licenseService.ValidateLicenseAsync(key);
            return isValid ? Results.Ok("License is valid.") : Results.BadRequest("Invalid or expired license.");
        }).RequireAuthorization();
        ;

        // Revoke License
        app.MapDelete("/revoke/{key}", async (LicenseService licenseService, string key) =>
        {
            var isRevoked = await licenseService.RevokeLicenseAsync(key);
            return isRevoked ? Results.Ok("License revoked successfully.") : Results.BadRequest("License not found.");
        }).RequireAuthorization();
        ;

        app.Run();
    }
}
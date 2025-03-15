using Microsoft.AspNetCore.RateLimiting;

namespace codewave_roots_licence_server.Modules;

public static class RateLimitModule
{
    public static IServiceCollection AddRateLimitModule(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed-window", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromMinutes(1); // 1-minute time window
                limiterOptions.PermitLimit = 10; // Allow 10 requests per minute
                limiterOptions.QueueLimit = 2; // Queue up to 2 requests
            });
        });

        return services;
    }
}
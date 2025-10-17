using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace AccountService.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckExtension(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
    public static IEndpointRouteBuilder UseHealthCheckExtension(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    details = report.Entries.Select(e => new
                    {
                        key = e.Key,
                        value = new
                        {
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.TotalMilliseconds
                        }
                    }),
                    duration = report.TotalDuration.TotalMilliseconds
                });
                await context.Response.WriteAsync(result);
            }

        }).RequireAuthorization();
        return app;
    }
}

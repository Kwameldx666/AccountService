using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

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

app.MapGet("/info", () => Results.Json(new
{
    message = "Welcome to Account Service API",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
}))
    .WithName("GetInfo")
    .WithOpenApi(op => new(op)
    {
        Summary = "Get basic service info",
        Description = "Returns a simple message, version, and environment details for the Account Service."
    });

app.MapControllers();

app.Run();

using AccountService.Api.Extensions;
using AccountService.Application.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddSwaggerDocumentation()
    .AddHealthCheckExtension()
    .AddApplicationServices()
    .AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();
app.UseHealthCheckExtension();

app.Run();

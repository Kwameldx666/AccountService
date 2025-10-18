using AccountService.Api.Extensions;
using AccountService.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddSwaggerDocumentation()
    .AddHealthCheckExtension()
    .AddApplicationServices();



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

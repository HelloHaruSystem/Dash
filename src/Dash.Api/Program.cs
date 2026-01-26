using Dash.Api.Features.Authentication;
using Dash.Infrastructure.DependencyInjection;
using Dash.Application.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add Infrastructure (DbContext, Options, etc)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application (like services)
builder.Services.AddApplication();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapAuthEndpoints();
app.MapGet("/", () => TypedResults.Ok("Hello, World!"));

app.Run();

// Make this class public instead of internal
// So it is testable
public partial class Program { }

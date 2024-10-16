using LoggingListener.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health/ready", () => "Healthy and Ready.");

app.MapGet("/health/live", () => "Healthy and Live").WithOpenApi();

app.MapGet("/api/test", () => $"Extension is working. Current time is {DateTime.Now:O}");

app.MapPost("/api/listeners/EntityCreated", (EntityCreated entityCreated, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to EntityCreated: {@EntityCreated}", entityCreated);
    return Results.NoContent();
}).WithOpenApi();

app.MapPost("/api/listeners/EntityUpdated", (EntityUpdated entityUpdated, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to EntityUpdated: {@EntityUpdated}, {@Fields}", entityUpdated, entityUpdated.Fields);
    return Results.NoContent();
}).WithOpenApi();

app.MapPost("/api/listeners/EntityDeleted", (EntityDeleted entityDeleted, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to EntityDeleted: {@EntityDeleted}", entityDeleted);
    return Results.NoContent();
}).WithOpenApi();

app.MapPost("/api/listeners/LinkCreated", (LinkCreated linkCreated, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to LinkCreated: {@LinkCreated}", linkCreated);
    return Results.NoContent();
}).WithOpenApi();

app.MapPost("/api/listeners/LinkDeleted", (LinkDeleted linkDeleted, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to LinkDeleted: {@LinkDeleted}", linkDeleted);
    return Results.NoContent();
}).WithOpenApi();

app.Run();
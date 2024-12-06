# Develop a preprocessing extension in .NET using Visual Studio 2022

## Prerequisites

This is a continuation of [develop an extension in .NET using Visual Studio 2022 with health and test endpoints](develop-dotnet-extension-using-visual-studio.md).

## Steps

### Add NuGet References for OpenAPI

To simplify testing, add these NuGet packages:
[Microsoft.AspNetCore.OpenApi](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/) and
[Swashbuckle.AspNetCore](https://www.nuget.org/packages/swashbuckle.AspNetCore/)

### Add OpenAPI to Your Code

* **Register OpenAPI services**: In **Program.cs** before `var app = builder.Build();` add the following:

  ```csharp
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();
  ```

* **Enable SwaggerUI**: just after `var app = builder.Build();` add the following:

  ```csharp
  app.UseSwagger();
  app.UseSwaggerUI();
  ```

* **Annotate Endpoints**: After each endpoint add `.WithOpenApi()`.

  Example:

  ```csharp
  app.MapGet("/health/ready", () => "Healthy and Ready.").WithOpenApi();
  ```

The resulting **Program.cs** looks like this:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health/ready", () => "Healthy and Ready.").WithOpenApi();

app.MapGet("/health/live", () => "Healthy and Live").WithOpenApi();

app.MapGet("/api/test", () => $"Test is working. Current time is {DateTime.Now:O}").WithOpenApi();

app.Run();
```

### Automatically launch the swagger page when starting the extension locally

Open **Properties\launchSettings.json** and replace the `launchUrl` value with the following: `"{Scheme}://{ServiceHost}:{ServicePort}**/swagger**"`.

The **Properties\launchSettings.json** file now looks like this:

```json
{
  "profiles": {
    "Container (Dockerfile)": {
    "commandName": "Docker",
    "launchBrowser": true,
    "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}/swagger",
    "environmentVariables": {
      "ASPNETCORE_URLS": "http://*:5005"
    },
    "httpPort": 5005
    }
  },
  "$schema": "http://json.schemastore.org/launchsettings.json"
}
```

### Test OpenAPI via Swagger

Build the project, and run it. The browser will open at **/swagger**.

Expand **GET /api/test**, click **Try it out**, then click **Execute** to test.

![Execute the test endpoint in swagger](../../screenshots/visual-studio/swagger-api-test-execute.png)

Stop Debugging.

### Listen to Entity Updates and Log Information

* **Add necessary using statements**: In **Program.cs** add the following at top: `using Microsoft.AspNetCore.Mvc;`.

* **Implement the EntityUpdated listener endpoint**

  This will enable the program to listen to entity updated events and perform any action(s).

  Before `app.Run();` add the following:

  ```csharp
  app.MapPost("/api/preprocessors/UpdateEntity", (Entity entity, [FromServices] ILogger<Program> logger) =>
  {
      logger.LogInformation("Received request to UpdateEntity: {@Entity}, {@Fields}", entity, entity.Fields);

      if (entity.Fields.Any(field => field.Data?.ToString() is "cancel"))
      {
          logger.LogInformation("Entity update was cancelled by preprocessor due to field with data 'cancel'.");
          return Results.UnprocessableEntity(new ValidationProblemDetails
          {
              Title = "Entity update was cancelled by preprocessor.",
              Detail = "Entity contains a field with data 'cancel'."
          });
      }

      if (entity.Fields.All(field => field.Data?.ToString() is not "update me"))
      {
          logger.LogInformation("Entity was not updated by preprocessor as no fields with data 'update me' were found.");
          return Results.NoContent();
      }

      logger.LogInformation("Entity was updated by preprocessor.");
      return Results.Ok(entity with { 
          Fields = entity.Fields.Select(field => field with { Data = field.Data?.ToString() is "update me" ? "Updated by preprocessor" : field.Data }).ToList() 
      });
  }).WithOpenApi();
  ```

* **Add the types** required for EntityUpdated

  Add the records that will enable strongly typed listener endpoints at the end of the file.

  ```csharp
  internal sealed record EntityUpdated
  {
    public required int Id { get; init; }

    public required string EntityTypeId { get; init; }

    public required string? FieldSetId { get; init; }

    public required IReadOnlyCollection<FieldModel> Fields { get; init; }

    public required int SegmentId { get; init; }
  }

  internal sealed record FieldModel
  {
    public required string FieldTypeId { get; init; }

    public object? Data { get; init; }
  }
  ```

The resulting **Program.cs** looks like this:

```csharp
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health/ready", () => "Healthy and Ready.").WithOpenApi();

app.MapGet("/health/live", () => "Healthy and Live").WithOpenApi();

app.MapGet("/api/test", () => $"Test is working. Current time is {DateTime.Now:O}").WithOpenApi();

app.MapPost("/api/preprocessors/UpdateEntity", (EntityUpdated entity, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to UpdateEntity: {@Entity}, {@Fields}", entity, entity.Fields);

    if (entity.Fields.Any(field => field.Data?.ToString() is "cancel"))
    {
        logger.LogInformation("Entity update was cancelled by preprocessor due to field with data 'cancel'.");
        return Results.UnprocessableEntity(new ValidationProblemDetails
        {
            Title = "Entity update was cancelled by preprocessor.",
            Detail = "Entity contains a field with data 'cancel'."
        });
    }

    if (entity.Fields.All(field => field.Data?.ToString() is not "update me"))
    {
        logger.LogInformation("Entity was not updated by preprocessor as no fields with data 'update me' were found.");
        return Results.NoContent();
    }

    logger.LogInformation("Entity was updated by preprocessor.");
    return Results.Ok(entity with { 
        Fields = entity.Fields.Select(field => field with { Data = field.Data?.ToString() is "update me" ? "Updated by preprocessor" : field.Data }).ToList() 
    });
}).WithOpenApi();

app.Run();

internal sealed record EntityUpdated
{
  public required int Id { get; init; }

  public required string EntityTypeId { get; init; }

  public required string? FieldSetId { get; init; }

  public required IReadOnlyCollection<FieldModel> Fields { get; init; }

  public required int SegmentId { get; init; }
}

internal sealed record FieldModel
{
  public required string FieldTypeId { get; init; }

  public object? Data { get; init; }
}
```

### Run the Container and Call the Endpoint

Build the project, and run it. The browser will open at `/swagger`.

Expand **POST /api/listeners/EntityUpdated**, click **Try it out**, and **Execute**.

View the log output where the container is running.

> **Note:**  
> The log can be seen both in _Output -> Show output from Debug_ and in _Containers -> Solution Containers -> first-example (ExampleProject) -> Logs_

![Container logs while running in visual studio](../../screenshots/visual-studio/container-logs-while-debugging-in-visual-studio.png)

Stop Debugging.

### Push the Docker Image

Go to **Build -> Publish ExampleProject -> Publish**

### Refresh the Docker Image Reference

In the Augmenta UI, navigate to the docker image reference that was previously created.

![Sync Docker Image Reference](../../screenshots/augmenta-ui/sync-docker-image-reference.png)

Click on **Sync Docker Image Reference**. This will fetch the latest image from the container registry and restart the extension that use the image.

### Update an Entity

Use the Portal or inriver REST API to change an entity's field value.

### Check the Logs

In the Augmenta UI, browse to the logs for your extension to view the results.

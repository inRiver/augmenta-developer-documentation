# Develop a listener extension in .NET

## Prerequisites

This is a continuation of [develop an extension in .NET with health and test endpoints](develop-dotnet-extension.md).

## Steps

### Add NuGet References for OpenAPI

To simplify testing, add these NuGet packages:
[Microsoft.AspNetCore.OpenApi](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/) and
[Swashbuckle.AspNetCore](https://www.nuget.org/packages/swashbuckle.AspNetCore/)

Run these commands:

```sh
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Swashbuckle.AspNetCore
```

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

### Test OpenAPI via Swagger

1. [Build the Docker image](develop-dotnet-extension.md#build-and-tag-the-docker-image)
2. [Run the container](develop-dotnet-extension.md#run-the-container-locally)
3. Open <http://localhost:5005/swagger/> in your browser.
4. Expand `GET /api/test`, click **Try it out**, then click **Execute** to test.

![Execute the test endpoint in swagger](../../screenshots/visual-studio/swagger-api-test-execute.png)

### Listen to Entity Updates and Log Information

* **Add necessary using statements**: In **Program.cs** add the following at top: `using Microsoft.AspNetCore.Mvc;`.

* **Implement the EntityUpdated listener endpoint**

  This will enable the program to listen to entity updated events and perform any action(s).

  Before `app.Run();` add the following:

  ```csharp
  app.MapPost("/api/listeners/EntityUpdated", (EntityUpdated entityUpdated, [FromServices] ILogger<Program> logger) =>
  {
      logger.LogInformation("Received request to EntityUpdated: {@EntityUpdated}, {@Fields}", entityUpdated, entityUpdated.Fields);
      return Results.NoContent();
  }).WithOpenApi();
  ```

* **Add the types** required for EntityUpdated

  Add the records that will enable strongly typed listener endpoints at the end of the file.

  ```csharp
  internal sealed record EntityUpdated
  {
      /// <summary>
      ///     Gets the unique ID of the entity that was updated.
      /// </summary>
      public required Guid Id { get; init; }

      /// <summary>
      ///     Gets the numeric ID of the entity that was updated.
      /// </summary>
      public required int NumericId { get; init; }

      /// <summary>
      ///     Get the fields that have been updated.
      /// </summary>
      public required FieldModelBase[] Fields { get; init; }

      /// <summary>
      ///     Gets the time when the entity was updated.
      /// </summary>
      public required DateTime Timestamp { get; init; }
  }

  internal sealed record FieldModelBase
  {
      /// <summary>
      ///     Gets the field type ID.
      /// </summary>
      public required string FieldTypeId { get; init; }
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

app.MapPost("/api/listeners/EntityUpdated", (EntityUpdated entityUpdated, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to EntityUpdated: {@EntityUpdated}, {@Fields}", entityUpdated, entityUpdated.Fields);
    return Results.NoContent();
}).WithOpenApi();

app.Run();

internal sealed record EntityUpdated
{
    /// <summary>
    ///     Gets the unique ID of the entity that was updated.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets the numeric ID of the entity that was updated.
    /// </summary>
    public required int NumericId { get; init; }

    /// <summary>
    ///     Get the fields that have been updated.
    /// </summary>
    public required FieldModelBase[] Fields { get; init; }

    /// <summary>
    ///     Gets the time when the entity was updated.
    /// </summary>
    public required DateTime Timestamp { get; init; }
}

internal sealed record FieldModelBase
{
    /// <summary>
    ///     Gets the field type ID.
    /// </summary>
    public required string FieldTypeId { get; init; }
}
```

### Run the Container and Call the Endpoint

1. Build the Docker image and run the container.
2. Open <http://localhost:5005/swagger/>.
3. Expand `POST /api/listeners/EntityUpdated`, click **Try it out**, and **Execute**.
4. View the log output in the terminal where the container is running.

### Push the Docker Image

Follow the steps in [Tag and push your image to a container registry](../tag-and-push-to-container-registry.md) to push the Docker image.

### Refresh the Docker Image Reference

In the Augmenta UI, navigate to the docker image reference that was previously created.

![Sync Docker Image Reference](../../screenshots/augmenta-ui/sync-docker-image-reference.png)

Click on **Sync Docker Image Reference**. This will fetch the latest image from the container registry and restart the extension that use the image.

### Update an Entity

Use the Portal or inriver REST API to change an entity's field value.

### Check the Logs

In the Augmenta UI, browse to the logs for your extension to view the results.

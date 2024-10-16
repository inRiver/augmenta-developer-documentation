# Use the inriver REST API from an extension

## Prerequisites

This is a continuation of [develop an extension in .NET with health and test endpoints](develop-dotnet-extension.md).

## Steps

### Retrieve API URL and API Key from Extension Settings

Instead of hardcoding the URL and API key, you can retrieve them from the extension's environment variables.
These settings are prefixed with `SETTING_` and are base64-encoded.

* **Add necessary using statements**: In **Program.cs** add the following at top: `using System.Text;` and `using Microsoft.AspNetCore.Mvc;`.

* **Register a `HttpClient`**: After `var builder = WebApplication.CreateBuilder(args);` add an `HttpClient` for calling the inriver REST API, using settings for the `API_URL` and `API_KEY`.

    ```csharp
    builder.Services.AddHttpClient("iPMC Rest Api client", client =>
    {
        client.BaseAddress = new Uri(GetExtensionSettingValue("API_URL"), UriKind.Absolute);
        client.DefaultRequestHeaders.Add("X-inRiver-APIKey", GetExtensionSettingValue("API_KEY"));
    });
    ```

* **Call the inriver REST API when the test endpoint is called**: Make the **test** endpoint async, enable the endpoint to call the inriver REST API, retrieve entity types, and return the entity types.

    ```csharp
    app.MapGet("/api/test", async (
        [FromServices] IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken) =>
    {
        var client = httpClientFactory.CreateClient("iPMC Rest Api client");
        var entityTypesResponseMessage = await client.GetAsync("api/v1.0.1/model/entitytypes", cancellationToken);
        var allEntityTypes = await entityTypesResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return $"All entity types: {allEntityTypes}";
    });
    ```

* **Retrieve environment variables**: Use the `GetExtensionSettingValue` method to base64-decode the `API_URL` and `API_KEY` environment variables, which must have the `SETTING_` prefix.

    > **Note:**  
    > After `app.Run();` we need to add `return;` to add this static method.

    ```csharp
    return;

    static string GetExtensionSettingValue(string settingName)
    {
        var environmentVariableName = "SETTING_" + settingName;
        var settingValueEncoded = Environment.GetEnvironmentVariable(environmentVariableName)
                                ?? throw new InvalidOperationException($"Missing environment variable: {environmentVariableName}");
        return Encoding.UTF8.GetString(Convert.FromBase64String(settingValueEncoded));
    }
    ```

The resulting **Program.cs** looks like this:

```csharp
using System.Text;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("iPMC Rest Api client", client =>
{
    client.BaseAddress = new Uri(GetExtensionSettingValue("API_URL"), UriKind.Absolute);
    client.DefaultRequestHeaders.Add("X-inRiver-APIKey", GetExtensionSettingValue("API_KEY"));
});

var app = builder.Build();

app.MapGet("/health/ready", () => "Healthy and Ready.");

app.MapGet("/health/live", () => "Healthy and Live");

app.MapGet("/api/test", async (
    [FromServices] IHttpClientFactory httpClientFactory,
    CancellationToken cancellationToken) =>
{
    var client = httpClientFactory.CreateClient("iPMC Rest Api client");
    var entityTypesResponseMessage = await client.GetAsync("api/v1.0.1/model/entitytypes", cancellationToken);
    var allEntityTypes = await entityTypesResponseMessage.Content.ReadAsStringAsync(cancellationToken);

    return $"All entity types: {allEntityTypes}";
});

app.Run();
return;

static string GetExtensionSettingValue(string settingName)
{
    var environmentVariableName = "SETTING_" + settingName;
    var settingValueEncoded = Environment.GetEnvironmentVariable(environmentVariableName)
                              ?? throw new InvalidOperationException($"Missing environment variable: {environmentVariableName}");
    return Encoding.UTF8.GetString(Convert.FromBase64String(settingValueEncoded));
}
```

### Build the docker image

Run the following command:

```sh
docker build -t example-image .
```

### Run the container locally and provide it with environment variables

> **Note:**  
> You can use an online Base64 encoder, such as [base64encode.org](https://base64encode.org), to encode your API key and URL before adding them as environment variables.
>
> If you don't have an API key for the inriver REST API, create one in the Control Center.

When running the container, along with `ASPNETCORE_URLS`, pass the environment variables for the API key and URL.

Run the following command:

```sh
docker run --name my-extension -e ASPNETCORE_URLS="http://*:5005" -e SETTING_API_KEY="bXktYXBpLWtleQ==" -e SETTING_API_URL="aHR0cHM6Ly9hcGktcHJvZDFhLXVzZS5wcm9kdWN0bWFya2V0aW5nY2xvdWQuY29tLw==" -p 5005:5005 --rm example-image
```

> **Note**:  
> These environment variables are not part of the Docker image. They are provided at runtime when the container is started via command line or later by Augmenta from the extension settings.

### Push the Docker Image

Follow the steps in [Tag and push your image to a container registry](../tag-and-push-to-container-registry.md) to push the Docker image.

### Create Extension Settings in Augmenta

Navigate to the Extension Settings in the Augmenta UI.

Create the following extension settings:

* **Setting 1**:
  * **Name**: `API_URL`
  * **Value**: `<https://api-prod1a-euw.productmarketingcloud.com>` or `<https://api-prod1a-use.productmarketingcloud.com>`
* **Setting 2**:
  * **Name**: `API_KEY`
  * **Value**: Your inriver REST API key

### Refresh the Docker Image Reference

In the Augmenta UI, navigate to the docker image reference that was previously created.

Click on "Sync Docker Image Reference." This will fetch the latest image from the container registry and restart the extension that use the image.

This will restart the extension. After restart the extension will have access to the environment variables created in the previous step.

### Test the extension

1. Select "Test" and click the "Test" button.
2. The response should be: "All entity types:..."

# Develop a scheduled extension in .NET using Visual Studio 2022

## Prerequisites

Make sure you have the following installed:

* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Docker desktop](https://www.docker.com/products/docker-desktop/)
* Visual Studio 2022 with web "module"

## Steps

### Create a New Console App project

* Open Visual Studio 2022

* Select **Create a new project**, and select `Console App` template.

  Name the project `ScheduledExampleProject`, Uncheck the `Place solution and project in the same directory` and name the solution `ScheduledExampleSolution`

  Choose `.Net 8` as the framework, select `Enable container support` and specify `Linux` as Container OS, and `Dockerfile` as Container build type.

  Complete the project setup.

### Update Docker support

Open the **Dockerfile** and make the following changes:

* Replace `FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base` with  
  `FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine-amd64 AS base`  
  since Augmenta runs on the amd64 architecture, and alpine images are lightweight.

* Replace `FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build` with
  `FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine-amd64 AS build`
  since Augmenta runs on the amd64 architecture, and alpine images are lightweight.

The resulting **Dockerfile** looks like this:

```Docker
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine-amd64 AS base
USER app
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine-amd64 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ScheduledExampleProject/ScheduledExampleProject.csproj", "ScheduledExampleProject/"]
RUN dotnet restore "./ScheduledExampleProject/ScheduledExampleProject.csproj"
COPY . .
WORKDIR "/src/ScheduledExampleProject"
RUN dotnet build "./ScheduledExampleProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ScheduledExampleProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScheduledExampleProject.dll"]
```

### Modify logic

Open **Program.cs** and replace the content with the following code:

```csharp
Console.WriteLine($"Extension is running. Current time is {DateTime.Now:O}");
```

As you can see, a scheduled extension does not expose a web api. Instead, it is expected to execute its logic and exit.

### Run the container locally from Visual Studio

Build the project, and run it. This will start the extension locally, and _Extension is running. Current time is..._ should be written to the console. The result can be seen in _*_Output -> Show output from: Debug_.

Congratulations. Your first scheduled extension just ran locally.

### Optional: specify name of the image

If you want to specify a different name of your image than `scheduledexampleproject`, then modify the ScheduledExampleProject.csproj to include this line under a `<PropertyGroup>`:

```xml
<DockerfileTag>first-scheduled-example</DockerfileTag>
```

The PropertyGroup will look like this:

```xml
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
    <DockerfileTag>first-scheduled-example</DockerfileTag>
  </PropertyGroup>
```

### Create a container registry

If you do not already have a container registry setup, follow [Setup a container registry and sign in](../tag-and-push-to-container-registry.md#setup-a-container-registry-and-sign-in) until [Create an access token](../tag-and-push-to-container-registry.md#create-an-access-token).

### Create a publish profile

In visual studio select Build -> Publish Selection

Select **Add a publish profile**.

Select **Docker Container Registry** -> **Docker Hub**

Enter your credentials with your username and use your personal access token as the password.

For the container build, specify Docker Desktop.

### Build and publish the Docker image

> **Note:**  
> The name of the image will be the name of the repository in docker hub.
>
> The repository in **Docker Hub** will be **public** (not private) when you publish the image since it will create a new repository.

Publish the docker image.

This will build the image locally and push it to the container registry.

### Reference the docker image in Augmenta from the container registry

1. Go to [Augmenta](https://augmenta-dev1a-euw.inriver.io/) and log in.
2. Select a customer, then an environment.
3. Navigate to Docker Image References.
4. Create a new Docker Image Reference, and enter the following:
    * **Image Name:** _\<my-username>/\<repository-name>_
    * **Image Tag:** `latest`

    and under Docker Registry specify

    * **Url:** _the url to your registry_ (example if you are using docker hub: _registry.hub.docker.com_)
    * **Username:** _your user name to the registry_
    * **Password:** _your password or token to the registry_

### Create an extension based on the docker image

From here create a new extension and fill in the details:

* **Unique ID:** `first-scheduled-extension`
* **Docker Image Reference:** (pre-filled)
* **Extension Type:** `ScheduledExtension`
* **Cron schedule:** `*/5***`
* **Enabled:** Checked

### Check the Logs

> **Note:**  
> Scheduled extensions can not be tested like the other types of extensions.

In the Augmenta UI, browse to the logs for your extension to verify that the extension executes as expected.

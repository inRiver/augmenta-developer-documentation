# Develop a scheduled extension in .NET

## Prerequisites

Make sure you have the following installed:

* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Docker desktop](https://www.docker.com/products/docker-desktop/)
* A code editor of your choice
* A working folder for your project (for example: `augmenta-test`)

## Steps

### Create a New Console Application project

Open your terminal, create the working folder if you haven’t already, and navigate into it. Then, run the following command to create a new Console Application project in a subdirectory named `ScheduledExampleProject`:

```sh
dotnet new console -n ScheduledExampleProject
```

### Add Docker support

Navigate to the `ScheduledExampleProject` folder:

```sh
cd ScheduledExampleProject
```

Create a new file named **Dockerfile** (without an extension) and add the following content:

```docker
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine-amd64 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine-amd64 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ScheduledExampleProject.csproj", "."]
RUN dotnet restore "./ScheduledExampleProject.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./ScheduledExampleProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ScheduledExampleProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

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

### Build and tag the docker image

To build and tag the image, make sure Docker is running and run the following command:

```sh
docker build -t scheduled-example-image .
```

### Run the container locally

Run the following command to start the container locally:

```sh
docker run --name my-scheduled-extension --rm scheduled-example-image
```

This will start the extension locally, and _Extension is running. Current time is..._ should be written to the console.

Congratulations. Your scheduled extension just ran locally.

### Tag the image and push to a container registry

Perform the steps described in [Tag and push your image to a container registry](../tag-and-push-to-container-registry.md)

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

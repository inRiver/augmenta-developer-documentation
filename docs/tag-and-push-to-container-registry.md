# Tag and push your image to a container registry

## Prerequisites

You have [setup a container registry in the cloud](setup-a-container-registry-in-the-cloud.md).

This is a continuation of built an image locally.

This guide expects that the image is called _example-image_.

## Steps

### Login

You have a container registry in the cloud. (See [Prerequisites](#prerequisites))

#### Login in using the terminal

Open a terminal window and login.

> **Note:**  
> This example uses `my-username` as the Docker Hub account name. Replace this with your own Docker Hub account name. E.g. _johndoe1911_.

```sh
docker login -u my-username
```

When prompted for a password, enter your personal access token.

### Tag the docker image

To tag the image (which allows you to push it to a container registry later), run the following command:

> **Note:**  
> Replace below `<my-username>/<repository-name>` with your Docker Hub account name and a repository name of your choice. E.g. _johndoe1911/my-example_.

```sh
docker tag example-image <my-username>/<repository-name>:latest
```

### Push the tagged image to a container registry

Ensure that you are [logged in to the container registry](#login-in-using-the-terminal) from and have sufficient privileges to push to the container registry.

Once logged in run this command:

> **Note:**  
> Replace all occurrences in these guides of `<my-username>/<repository-name>` with your Docker Hub account name and repository name.

```sh
docker push <my-username>/<repository-name>:latest
```

### Next steps

Proceed to [Run the extension in Augmenta](run-extension-in-augmenta.md)

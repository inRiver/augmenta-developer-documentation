# Develop an extension in Node.js

## Prerequisites

Make sure you have the following installed:

* [Node.js](https://nodejs.org/en/download/package-manager/current)
* [Docker desktop](https://www.docker.com/products/docker-desktop/)
* A code editor of your choice
* A working folder for your project (for example: `augmenta-test`)

## Steps

### Create a new Node.js project

Open your terminal, create the working folder if you haven’t already, and navigate into it. Then, run the following commands to create a new Node.js application in a subdirectory named `example-project`:

```sh
mkdir example-project
cd example-project
npm init
```

Accept all default values.

### Install Express

Run the following commands to the install express package:

```sh
npm i --save express
```

### Add Docker support

Create a new file named **Dockerfile** (without an extension) and add the following content:

```docker
FROM node:22-alpine

WORKDIR /usr/src/app

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 5005

CMD [ "node", "index.js" ]
```

### Create an express rest api with the required endpoints

Create a new file named **index.js** and add the following content:

```javascript
import express from 'express';

const app = express();

app.get('/health/ready', (req, res) => {
  res.send('Healthy and Ready.');
});

app.get('/health/live', (req, res) => {
  res.send('Healthy and Live');
});

app.get('/api/test', async (req, res) => {
  res.send(`Test is working. Current time is ${new Date().toISOString()}`);
});

const PORT = process.env.PORT ?? 5005;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
```

### Build and tag the docker image

To build and tag the image, make sure Docker is running and run the following command:

```sh
docker build -t example-image .
```

### Run the container locally

Run the following command to start the container locally:

```sh
docker run -it --init --name my-extension -p 5005:5005 --rm example-image
```

> **Note:**  
> If you encounter an error about port allocation, stop any program using port 5005 and try again.

This will start the extension locally from the docker image [built in the previous step](#build-and-tag-the-docker-image), and port 5005 will be accessible on localhost.

### Call the test endpoint

You can test the endpoint by opening a browser and navigating to:
<http://localhost:5005/api/test> or by using `curl` in the terminal:

```sh
curl http://localhost:5005/api/test
```

Congratulations. Your first extension is now running locally.

### Prepare for next steps

Stop the running container that was started in [Run the container locally](#run-the-container-locally) by pressing Ctrl+C in that window.

* If you're using a computer with an ARM-based CPU (such as a newer Mac with the M1 chip), you'll need to build the Docker image for amd64 architecture in order for it to run in Augmenta. Run the following command before pushing the image:

  ```sh
  docker build --platform linux/amd64 -t example-image .
  ```

### Next steps

Proceed to [Tag and push your image to a container registry](../tag-and-push-to-container-registry.md)

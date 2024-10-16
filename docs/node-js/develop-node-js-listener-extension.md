# Develop a listener extension in Node.js

## Prerequisites

This is a continuation of [developed a Node.js extension with health and test endpoints.](develop-node-js-extension.md).

## Steps

Open **index.js** and replace it with:

### Listen to Entity Updates and Log Information

Update **index.js** with:

```javascript
import express from 'express';

const app = express();

app.use(express.json());

app.get('/health/ready', (req, res) => {
  res.send('Healthy and Ready.');
});

app.get('/health/live', (req, res) => {
  res.send('Healthy and Live');
});

app.get('/api/test', async (req, res) => {
  res.send(`Test is working. Current time is ${new Date().toISOString()}`);
});

app.post('/api/listeners/EntityUpdated', (req, res) => {
  console.log('Received EntityUpdated request:', req.body);
  res.status(204).send();
});

const PORT = process.env.PORT ?? 5005;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
```

Breakdown of the above changes:

* Add support for handling json:

    After `const app = express();` add

    ```javascript
    app.use(express.json());
    ```

* Implement the `EntityUpdated` listener endpoint

    ```javascript
    app.post('/api/listeners/EntityUpdated', (req, res) => {
      console.log('Received EntityUpdated request:', req.body);
      res.status(204).send();
    });
    ```

### Run the Container and Call the Endpoint

1. [Build the Docker image](develop-node-js-extension.md#build-and-tag-the-docker-image)
2. [Run the container](develop-node-js-extension.md#run-the-container-locally)
3. Call the endpoint by running this command:

    * For Windows users using PowerShell:

        ```powershell
        Invoke-RestMethod -Method Post -Uri 'http://localhost:5005/api/listeners/EntityUpdated' `
        -Headers @{ `
            accept = '*/*'; `
            'Content-Type' = 'application/json' `
        } `
        -Body (@{ `
            timestamp = '2024-12-31T19:00:00.000Z'; `
            id = '3fa85f64-5717-4562-b3fc-2c963f66afa6'; `
            numericId = 42; `
            fields = @(@{ `
            fieldTypeId = 'MyFieldTypeId' `
            }) } `
        | ConvertTo-Json)
        ```

    * For non Windows users, using bash:

        ```sh
        curl -X 'POST' \
        'http://localhost:5005/api/listeners/EntityUpdated' \
        -H 'accept: */*' \
        -H 'Content-Type: application/json' \
        -d '{
        "timestamp": "2024-12-31T19:00:00.000Z",
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "numericId": 42,
        "fields": [
            {
            "fieldTypeId": "MyFieldTypeId"
            }
        ]
        }'
        ```

4. View the log output in the terminal where the container is running.

### Push the Docker Image

Follow the steps in [Tag and push your image to a container registry](../tag-and-push-to-container-registry.md) to push the Docker image.

### Refresh the Docker Image Reference

In the Augmenta UI, Click on "Sync Docker Image Reference." This will fetch the latest image from the container registry and restart the extension that use the image.

### Update an Entity

Use the Portal or inriver REST API to change an entity's field value.

### Check the Logs

In the Augmenta UI, browse to the logs for your extension to view the results.

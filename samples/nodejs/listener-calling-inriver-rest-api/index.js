import express from 'express';
import axios from 'axios';
import { Buffer } from 'buffer';

const app = express();
app.use(express.json({ type: ['application/json', 'application/cloudevents+json'] }));

function getExtensionSettingValue(settingName) {
    const environmentVariableName = `SETTING_${settingName}`;
    const settingValueEncoded = process.env[environmentVariableName];
    if (!settingValueEncoded) {
        throw new Error(`Missing environment variable: ${environmentVariableName}`);
    }
    return Buffer.from(settingValueEncoded, 'base64').toString('utf8');
}

app.get('/health/ready', (req, res) => {
  res.send('Healthy and Ready.');
});

app.get('/health/live', (req, res) => {
  res.send('Healthy and Live');
});

app.get('/api/test', async (req, res) => {
  res.send(`Test is working. Current time is ${new Date().toISOString()}`);
});

app.post('/api/listeners/EntityUpdated', async (req, res) => {
    console.log('Received EntityUpdated request:', JSON.stringify(req.body));
    let entityUpdated = req.body;

    try {
        const iPMCApiClient = axios.create({
            baseURL: getExtensionSettingValue("API_URL"),
            headers: {
                'X-inRiver-APIKey': getExtensionSettingValue("API_KEY"),
            },
        });

        const response = await iPMCApiClient.get(`api/v1.0.0/entities/${entityUpdated.numericId}/fieldvalues?fieldTypeIds=${entityUpdated.fields.map(field => field.fieldTypeId).join(',')}`);
        const changedFieldValues = response.data;

        console.log("Changed field values retrieved from inriver REST API:", JSON.stringify(changedFieldValues));

        res.sendStatus(204);
    } catch (error) {
        console.error("Error retrieving field values:", error);
        res.status(500).send("An error occurred while processing the request.");
    }
  });
  
const PORT = process.env.PORT ?? 5005;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});

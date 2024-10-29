# Node.js Samples

## [Call inriver REST API when entity is updated](./listener-calling-inriver-rest-api/)

This sample is a listener extension.
It contains an endpoint that is triggered by EntityUpdated event.
The endpoint handler calls the inriver REST API to get the changed field values of the updated entity, and logs the output.

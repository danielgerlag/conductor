# Making Http requests in your workflow

Let's define a workflow using the built-in `HttpRequest` step.

```http
POST /api/definition
Content-Type: application/json
```
```json
{
  "id": "MyHttpWorkflow",
  "steps": [
    {            
      "id": "Step1",
      "stepType": "HttpRequest",
      "inputs": {
        "BaseUrl": "\"http://demo7149346.mockable.io/\"",
        "Resource": "\"ping\"",
        "Headers": {
          "@Authorization": ""
        }
      },
      "outputs": {
        "ResponseCode": "step.ResponseCode",
        "ResponseBody": "step.ResponseBody"
      }
    }
  ]
}
```

or in Yaml format:
```http
POST /api/definition
Content-Type: application/yaml
```
```yaml
id: MyHttpWorkflow
steps:
- id: Step1
  stepType: HttpRequest
  inputs:
    BaseUrl: '"http://demo7149346.mockable.io/"'
    Resource: '"ping"'
    Headers:
      Content-Type: application/json
  outputs:
    ResponseCode: step.ResponseCode
    ResponseBody: step.ResponseBody
```

Now, lets test it by invoking a new instance of our workflow with an empty data object
```
POST /api/workflow/MyHttpWorkflow
Content-Type: application/json
```
```json
{}
```

Response:
```json
{
    "workflowId": "5d0ab0ff23576b61e4afbcfb",
    "data": {},
    "definitionId": "MyHttpWorkflow",
    "version": 1,
    "status": "Runnable",
}
```

Now the workflow should call the Http endpoint and record the response code and body in it's internal data object.  Let's inspect it with via the API.


```
GET /api/workflow/5d0ab0ff23576b61e4afbcfb
```

Response:
```json
{
    "workflowId": "5d0ab0ff23576b61e4afbcfb",
    "data": {
        "ResponseCode": 200,
        "ResponseBody": {
          "msg": "Hello world"
        }
    },
    "definitionId": "MyHttpWorkflow",
    "version": 1,
    "status": "Complete",
}
```

An example of a `POST` with two values, one static and one sourced from the internal data object of the workflow.

```yaml
id: MyHttpWorkflow
steps:
- id: Step1
  stepType: HttpRequest
  inputs:
    BaseUrl: '"http://demo7149346.mockable.io/"'
    Resource: '"pong"'
    Method: '"POST"'
    Body:
      Value1: 1
      '@Value2': data.Value2
  outputs:
    ResponseCode: step.ResponseCode
    ResponseBody: step.ResponseBody
```
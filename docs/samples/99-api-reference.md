## API Reference

### Definition API

#### Create or update a definition

We `POST` the definition to `api/definition` in either `YAML` or `JSON`.

```http
POST /api/definition
Content-Type: application/yaml
```
```yml
Id: Hello1
Steps:
- Id: Step1
  StepType: EmitLog
  NextStepId: Step2
  Inputs:
    Message: '"Hello world"'
    Level: '"Information"'
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: '"Goodbye!!!"'
    Level: '"Information"'
```

Posting to a definition ID that already exists, will create a second version of that workflow definition and all existing workflows that were started on the old verison, will continue on the old version but all workflows that are started after this will run on the new version.


### Workflow API

#### Start a workflow

To start a workflow, submit a `POST` to `/api/workflow/<<DefinitionId>>`, where the body of the request will be the initial data object passed to the new workflow instance.

```
POST /api/workflow/<<DefinitionId>>

```
Example: Start the `HelloWorld` workflow, with some custom data.
```
POST /api/workflow/HelloWorld
Content-Type: application/x-yaml
```
```yaml
CustomMessage: foobar
```

##### Response

```json
{
    "workflowId": "5d26ae05ec9ce50001bc9c2a",
    "data": {
        "CustomMessage": "foobar"
    },
    "definitionId": "HelloWorld",
    "version": 1,
    "status": "Runnable",
    "reference": null,
    "startTime": "2019-07-11T03:33:25.203Z",
    "endTime": null
}
```

#### Querying a workflow

If you have the `workflowId` that you get back when you start a workflow, you can query it's status via the API.

```
GET /api/workflow/<<WorkflowId>>
```

##### Response

```json
{
    "workflowId": "5d26ae05ec9ce50001bc9c2a",
    "data": {
        "CustomMessage": "foobar"
    },
    "definitionId": "HelloWorld",
    "version": 1,
    "status": "Runnable",
    "reference": null,
    "startTime": "2019-07-11T03:33:25.203Z",
    "endTime": null
}
```

#### Suspending a workflow

You can suspend a workflow with a `PUT`

```
PUT /api/workflow/<<WorkflowId>>/suspend
```


#### Resuming a workflow

You can resume a suspended a workflow with a `PUT`

```
PUT /api/workflow/<<WorkflowId>>/resume
```

#### Terminting a workflow

You can abort a workflow with a `DELETE`

```
DELETE /api/workflow/<<WorkflowId>>
```


### Event API

You can publish an event with a particular name and key and attach some data to all workflows that may be listening to it.  Use the event API.

** Currently, only scalar values are supported for the attached data **

```
POST /api/event/<<name>>/<<key>>
```
```
<<data>>
```


### Lambda API

Conductor also allows you to define lambda's or scripts that can be used within your workflows.  Currently, the only supported language is Python.  More languages will be implemented in the future.

#### Creating a lambda

The following call creates a lambda function called `add`, which is a Python script that sets c to a + b
```
POST /api/lambda/add
Content-Type: text/x-python
```
```python
c = a + b
```

#### Viewing a lambda

```
GET /api/lambda/<<id>>
```


### Diagnostic API

```
GET /api/info
```

## Introduction

Conductor lets you coordinate multiple services and scripts into workflows so you can build and update apps quickly. Workflows are made up of a series of steps, with an internal data object shared between steps to pass information around. Conductor automatically triggers and tracks each step, and retries when there are errors, so your application executes in order and as expected.


### Defining a workflow

We'll start by defining a simple workflow that will log "Hello world" as it's first step and then "Goodbye!!!" as it's second and final step.  We `POST` the definition to `api/definition` in either `YAML` or `JSON`.

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
or
```http
POST /api/definition
Content-Type: application/json
```
```yml
{
  "Id": "Hello1",
  "Steps": [
    {
      "Id": "Step1",
      "StepType": "EmitLog",
      "NextStepId": "Step2",
      "Inputs": {
        "Message": "\"Hello world\"",
        "Level": "\"Information\""
      }
    },
    {
      "Id": "Step2",
      "StepType": "EmitLog",
      "Inputs": {
        "Message": "\"Goodbye!!!\"",
        "Level": "\"Information\""
      }
    }
  ]
}
```

Now, lets test it by invoking a new instance of our workflow.
We do this with a `POST` to `/api/workflow/Hello1`
```
POST /api/workflow/Hello1
Content-Type: application/json
```
```json
{}
```

We can also rewrite our workflow to pass custom data to any input on any of it's steps.

```yml
Id: Hello2
Steps:
- Id: Step1
  StepType: EmitLog
  Inputs:
    Message: data["CustomMessage"]
    Level: '"Information"'
```

Now, when we start a new instance of the workflow, we also initialize it with some data.

```
POST /api/workflow/Hello2
Content-Type: application/json
```
```json
{
  "CustomMessage": "foobar"
}
```
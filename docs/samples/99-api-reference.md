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

#### Querying a workflow

#### Suspending a workflow

#### Resuming a workflow

### Event API


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

### Diagnostic API


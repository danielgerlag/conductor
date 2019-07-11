## Conductor

Conductor is a workflow server built upon [Workflow Core](https://github.com/danielgerlag/workflow-core) that enables you to coordinate multiple services and scripts into workflows so that you can rapidly create complex workflow applications. Workflows are made up of a series of steps, with an internal data object shared between them to pass information around.  Conductor automatically runs and tracks each step, and retries when there are errors.

Workflows are written in either JSON or YAML and then added to Conductor's internal registry via the definition API.  Then you use the workflow API to invoke them with or without custom data.

### Installation

Conductor is available as a Docker image - `danielgerlag/conductor`

```shell
> docker run danielgerlag/conductor ...
```
Conductor uses MongoDB as it's datastore, you will also need an instance of MongoDB in order to run Conductor.
If you wish to run a fleet of Conductor nodes, then you also need to have a Redis instance, which they will use as a backplane.  This is not required if you are only running one instance.

#### Environment Variables to configure

You can configure the database and Redis backplane by setting environment variables.
```
DBHOST: <<insert connection string to your MongoDB server>>
REDIS: <<insert connection string to your Redis server>> (optional)
```

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

Now, lets test it by invoking a new instance of our workflow.
We do this with a `POST` to `/api/workflow/Hello1`
```
POST /api/workflow/Hello1
```

We can also rewrite our workflow to pass custom data to any input on any of it's steps.

```yml
Id: Hello2
Steps:
- Id: Step1
  StepType: EmitLog
  Inputs:
    Message: data.CustomMessage
    Level: '"Information"'
```

Now, when we start a new instance of the workflow, we also initialize it with some data.

```
POST /api/workflow/Hello2
Content-Type: application/x-yaml
```
```yaml
CustomMessage: foobar
```

## Further reading
* [Introduction](docs/01-intro.md)
* [Workflow Primitives](docs/02-primitives.md)
* [Http requests](docs/03-http.md)
* [Lambdas](docs/04-lambda.md)
* [API Reference](docs/99-api-reference.md)
* [Roadmap](docs/roadmap.md)

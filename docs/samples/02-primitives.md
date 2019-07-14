## Workflow primitives

### Input / Output Expressions

When setting input and output values on your steps, you can either specify constant values or an expression.

```yaml
Inputs:
    Message: '"Hello world"'
```
All inline expressions are written in Python.

```yaml
Inputs:
    Message: '"Your value is " + str(data.MyValue)'
```

There are several variables that are available to your expression
* data
  This gives you access to the internal data object specific to your workflow.
  eg. If you passed an object that had a `MyValue` field when you invoked your workflow, you could access it like this
  ```python
  data.MyValue
  ```
* context
  This gives you access to some contextual information about your workflow.
  eg. Get the wotkflow id.
  ```python
  context.Workflow.Id
  ```
  eg. Get the current context item within a for loop
  ```python
  context.Item
  ```
* environment
  This gives you access to the system evironment variables.
  ```
  environment["VARIABLE_NAME"]
  ```

You can also pass in an object graph to a given input.

```yaml
Inputs:
    BaseUrl: '"http://demo7149346.mockable.io/"'
    Resource: '"pong"'
    Method: '"POST"'
    Body:
      Value1: 1
      Value2: 2
```

If you would like to have some of the properties on the object graph evaluated as an expression, simply prepend and @ and pass an expression string.

```yaml
Inputs:
    BaseUrl: '"http://demo7149346.mockable.io/"'
    Resource: '"pong"'
    Method: '"POST"'
    Body:
      Value1: 1
      '@Value2': data.Value2
```

#### Outputs

When mapping outputs from a step, the Key represents a field on your data object that is global to your workflow instance, and the expression allows you to map result values produced by the step after it has run.
eg. This sets `ResponseCode` and `ResponseBody` on your workflow wide data object to the corresponding values return by the step.
```yaml
Outputs:
  ResponseCode: step.ResponseCode
  ResponseBody: step.ResponseBody
```

### Error handling

TODO


### If

Use the `If` step type to branch on a condition expression.

```yml
Id: if-test
Steps:
- Id: Step1
  StepType: If
  NextStepId: Step2
  Inputs:
    Condition: 'data.MyValue > 3'
  Do:
  - - Id: Step1.1
      StepType: EmitLog
      NextStepId: Step1.2
      Inputs:
        Message: '"You value is greater than 3"'
    - Id: Step1.2
      StepType: EmitLog
      Inputs:
        Message: '"Thank you"'
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: '"Your value is " + str(data.MyValue)'
```

### While

Use the `While` step type to loop on a condition expression.

```yml
Id: while-test
Steps:
- Id: Step1
  StepType: While
  Inputs:
    Condition: 'data.Counter < 3'
  Do:
  - - Id: Step1.1
      StepType: NoOp
      Outputs:
        Counter: 'data.Counter + 1'
```

### ForEach

Use the `ForEach` step type to iterate on a collection expression.
Note: `ForEach` iterates in parallel.

```yml
Id: foreach-test
Steps:
- Id: Step1
  StepType: Foreach
  Inputs:
    Collection: '[1, 2, 3]'
  Do:
  - - Id: Step1.1
      StepType: EmitLog
      Inputs:
        Message: 'context.Item'    
```

### Parallel

Use the `Parallel` step type to branch into multiple parallel sequences.

```yml
Id: parallel-test
Steps:
- Id: Step1
  StepType: Parallel
  NextStepId: Step2
  Do:
  - - Id: Step1.1 #Branch 1
      StepType: EmitLog
      NextStepId: Step1.2
      Inputs:
        Message: '"Step 1.1"'
    - Id: Step1.2
      StepType: EmitLog
      Inputs:
        Message: '"Step 1.2"'
  - - Id: Step2.1 #Branch 2
      StepType: EmitLog
      NextStepId: Step2.2
      Inputs:
        Message: '"Step 2.1"'
    - Id: Step2.2
      StepType: EmitLog
      Inputs:
        Message: '"Step 2.2"'
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: '"done"'
```


### Delay

Use the `Delay` step type to wait a specified time based on an expresion.

```yml
Id: delay-test
Steps:
- Id: Step1
  StepType: Delay
  NextStepId: Step2
  Inputs:
    Period: '...'
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: '"done"'
```

### Recur

Use the `Recur` step type to trigger a recurring background sequence of steps.

```yml
Id: recur-test
Steps:
- Id: Step1
  StepType: Recur
  Inputs:
    Interval: '...'
    StopCondition: '...'
  Do:
  - - Id: Step1.1
      StepType: EmitLog
      Inputs:
        Message: '"doing recurring work..."'
```


### Schedule

Use the `Schedule` step type to schedule a future set of steps to execute after a specified interval.

```yml
Id: schedule-test
Steps:
- Id: Step1
  StepType: Schedule
  Inputs:
    Interval: '...'
  Do:
  - - Id: Step1.1
      StepType: EmitLog
      Inputs:
        Message: '"doing scheduled work..."'
```


### WaitFor

Use the `WaitFor` step type to pause your workflow and wait for an external event.

The following example will wait for an external event of name `my-event` with a key of `5`.  You can of course, choose a name and key based on data in the custom data object of your workflow instance, using a Python expression.

```yml
Id: waitfor-test
Steps:
- Id: Step1
  StepType: WaitFor
  NextStepId: Step2
  Inputs:
    EventName: '"my-event"'
    EventKey: '5'
  Outputs:
    EventData: step.EventData
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: 'data.EventData'
```

We can then use the event API to publish an event of a given name and key so that all workflows that are listening for it will be notified and recieve any data associated with the event.

First, let's start a new workflow instance.
```
POST /api/workflow/waitfor-test
{}
```
Response:
```json
{
    "workflowId": "5d274481ec9ce50001bc9c34",
    "data": {},
    "definitionId": "waitfor-test",
    "version": 2,
    "status": "Runnable",
    "reference": null,
    "startTime": "2019-07-11T14:15:29.86Z",
    "endTime": null
}
```

Then, let's publish an event with a name of `my-event` and a key of 5.  We'll also pass the string `"test"` as data on the event.

```
POST /api/event/my-event/5
```
```
"test"
```

This will notify all workflows that are waiting on this particular name/key combo and pass `"test"` to them.  The outcome of this particular example will store the data from the event in the workflow's own internal data object.

So, if we query the status of the workflow, we will see:
```
GET /api/workflow/5d274481ec9ce50001bc9c34
```
Response:
```json
{
    "workflowId": "5d274481ec9ce50001bc9c34",
    "data": {
        "EventData": "test"
    },
    "definitionId": "waitfor-test",
    "version": 2,
    "status": "Complete",
    "reference": null,
    "startTime": "2019-07-11T14:15:29.86Z",
    "endTime": "2019-07-11T14:15:30.185Z"
}
```


TODO: effective date
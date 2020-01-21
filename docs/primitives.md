# Input / Output Expressions

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

## Outputs

When mapping outputs from a step, the Key represents a field on your data object that is global to your workflow instance, and the expression allows you to map result values produced by the step after it has run.
eg. This sets `ResponseCode` and `ResponseBody` on your workflow wide data object to the corresponding values return by the step.
```yaml
Outputs:
  ResponseCode: step.ResponseCode
  ResponseBody: step.ResponseBody
```

## Branching

You can define multiple independent branches within your workflow and select one based on an expression value.
Hook up your branches via the `SelectNextStep` property, instead of a `NextStepId`.  The expressions will be matched to the step Ids listed in `SelectNextStep`, and the matching next step(s) will be scheduled to execute next.  If more then one step is matched, then the workflow will have multiple parallel paths.

```json
{
  "Id": "decide-workflow",
  "Version": 1,
  "Steps": [
    {
      "Id": "Start",
      "StepType": "Decide",
      "SelectNextStep": {
      	"A": "data.Value1 == 2",
      	"B": "data.Value1 == 3"
      }
    },    
    {
      "Id": "A",
      "StepType": "EmitLog",
      "Inputs": {
        "Message": "\"Hi from A!\""
      }
    },
    {
      "Id": "B",
      "StepType": "EmitLog",
      "Inputs": {
        "Message": "\"Hi from B!\""
      }
    }
  ]
}

```

```yaml
Id: decide-workflow
Version: 1
Steps:
- Id: Start
  StepType: Decide
  SelectNextStep:
    A: data.Value1 == 2
    B: data.Value1 == 3
- Id: A
  StepType: EmitLog
  Inputs:
    Message: '"Hi from A!"'
- Id: B
  StepType: EmitLog
  Inputs:
    Message: '"Hi from B!"'
```

# Error handling

TODO


# If

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

# While

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

# ForEach

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

# Parallel

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


# Delay

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

# Recur

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

# Schedule

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

# WaitFor

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


# Activity

An activity is defined as an item on an external queue of work, that a workflow can wait for.
Use the `Activity` step type to pause your workflow and wait for an external activity worker that you implement.


The following example will wait for an external activity of name `my-activity` and pass "Hello from the workflow" as the input data to the worker that will process this activity.  Once the worker submits a response, the workflow will continue and log the result from the worker. 

```yml
Id: activity-test
Steps:
- Id: Step1
  StepType: Activity
  NextStepId: Step2
  Inputs:
    ActivityName: '"my-activity"'
    Parameters: '"Hello from the workflow"'
  Outputs:
    ActivityResult: step.Result
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: 'data.ActivityResult'
```

We can then use the activity API to implement a worker to process "my-activity" activities.

First, let's start a new workflow instance.
```
POST /api/workflow/activity-test
{}
```
Response:
```json
{
    "workflowId": "5d274481ec9ce50001bc9c34",
    "data": {},
    "definitionId": "activity-test",
    "version": 1,
    "status": "Runnable",
    "reference": null,
    "startTime": "2019-07-11T14:15:29.86Z",
    "endTime": null
}
```

In our external worker process, we can fetch a waiting activity of an active workflow with the following request. 

```
GET /api/activity/my-activity?timeout=30
```

This will wait up to 30 seconds for some work be become ready to process, if there is no workflow waiting on the activity requested, then a `404 Not Found` will be returned.  If there is work waiting for that activity queue, then an exclusive token will be issued and the reponse will look as follows

```json
{
    "token": "eyJTdWJzY3JpcHRpb25JZCI6IjVlMD",
    "activityName": "my-activity",
    "parameters": "Hello from the workflow",
    "tokenExpiry": "9999-12-31T23:59:59.9999999"
}
```
* `token` An exclusive token is issued to the worker to use in future requests for this activity.
* `parameters` The input data that the workflow attached to this actvity. Can be any object.
* `tokenExpiry` When the token expires and the activity will be made available to other workers.


To submit a successful response to an activity and pass some response data back to the workflow in the body of the request

```
POST /api/activity/success/eyJTdWJzY3JpcHRpb25JZCI6IjVlMD
```
```
"Hello from the worker"
```

The workflow will now continue and the "Hello from the worker" string will be mapped to the `ActivityResult` field on the workflow data, and then logged in the final step.  The response data can be any object, not just scalar values.

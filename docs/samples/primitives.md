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

### If

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

```yml
Id: waitfor-test
Steps:
- Id: Step1
  StepType: WaitFor
  NextStepId: Step2
  Inputs:
    EventName: '...'
    EventKey: '...'
  Outputs:
    EventData: step.EventData
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: 'data.EventData'
```

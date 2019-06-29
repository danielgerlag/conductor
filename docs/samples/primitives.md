## Workflow primitives



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

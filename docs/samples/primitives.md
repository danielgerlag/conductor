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
  - - Id: Step1.2
      StepType: EmitLog
      NextStepId: Step2.2
      Inputs:
        Message: '"You value is greater than 3"'
    - Id: Step2.2
      StepType: EmitLog
      Inputs:
        Message: '"Thank you"'
- Id: Step2
  StepType: EmitLog
  Inputs:
    Message: '"Your value is " + str(data.MyValue)'
```
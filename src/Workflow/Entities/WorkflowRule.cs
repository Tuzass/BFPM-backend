using Workflow.Enums;

namespace Workflow.Entities;

public class WorkflowRule
{
    public int ID {get; private set;}
    public Trigger Trigger {get; private set;}
    public State State {get; private set;}

    public WorkflowRule(int id, Trigger trigger, State state)
    {
        ID = id;
        Trigger = trigger;
        State = state;
    }

    private WorkflowRule()
    {
        
    }
}

using Workflow.Enums;

namespace Workflow.Entities;

public class SystemAction
{
    public int ID {get; private set;}
    public ActionType Type {get; private set;}
    public ActionTarget Target {get; private set;}
    public State NewState {get; private set;}

    public SystemAction(int id, ActionType type, ActionTarget target, State newState)
    {
        ID = id;
        Type = type;
        Target = target;
        NewState = newState;
    }

    private SystemAction()
    {
        
    }
}

using Workflow.Enums;

namespace Workflow.Entities;

public class SystemAction
{
    public int ID {get; private set;}
    public ActionType ActionType {get; private set;}
    public ActionTarget ActionTarget {get; private set;}
    public State NewTargetState {get; private set;}

    public SystemAction(ActionType actionType, ActionTarget actionTarget, State newTargetState)
    {
        ActionType = actionType;
        ActionTarget = actionTarget;
        NewTargetState = newTargetState;
    }

    private SystemAction()
    {
        
    }
}

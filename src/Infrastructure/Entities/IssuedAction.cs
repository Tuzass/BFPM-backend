using Infrastructure.Enums;
using Workflow.Enums;

namespace Infrastructure.Entities;

public class IssuedAction
{
    public int ID {get; private set;}
    public Trigger Trigger {get; private set;}
    public ActionType ActionType {get; private set;}
    public DomainEntityType TargetType {get; private set;}
    public DomainEntityType? RelatedEntityType {get; private set;}
    public int TargetID {get; private set;}
    public int? RelatedEntityID {get; private set;}
    public Change? Change {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}
    public DateTime? ExecutedAt {get; private set;}
    public IssuedActionStatus Status {get; private set;}

    public IssuedAction(Trigger trigger, ActionType actionType, DomainEntityType targetType, DomainEntityType? relatedEntityType, int targetID, int? relatedEntityID, Change? change)
    {
        Trigger = trigger;
        ActionType = actionType;
        TargetType = targetType;
        RelatedEntityType = relatedEntityType;
        TargetID = targetID;
        RelatedEntityID = relatedEntityID;
        Change = change;
        CreatedAt = DateTime.UtcNow;
        ExecutedAt = null;
        Status = IssuedActionStatus.Pending;
    }

    private IssuedAction()
    {
        
    }

    public void Execute()
    {
        if (Status == IssuedActionStatus.Executed)
        {
            return;
        }

        Status = IssuedActionStatus.Executed;
        ExecutedAt = DateTime.UtcNow;
    }
}

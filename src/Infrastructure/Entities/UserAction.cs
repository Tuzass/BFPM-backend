using Infrastructure.Enums;
using Workflow.Enums;

namespace Infrastructure.Entities;

public class UserAction
{
    public int ID {get; private set;}
    public int UserID {get; private set;}
    public ActionType ActionType {get; private set;}
    public DomainEntityType TargetType {get; private set;}
    public DomainEntityType? RelatedEntityType {get; private set;}
    public int TargetID {get; private set;}
    public int? RelatedEntityID {get; private set;}
    public string? LocalChangeJSON {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}
    public DateTime? ProcessedAt {get; private set;}
    public UserActionStatus Status {get; private set;}

    public UserAction(int userID, ActionType actionType, DomainEntityType targetType, DomainEntityType? relatedEntityType, int targetID, int? relatedEntityID, string? localChangeJSON)
    {
        UserID = userID;
        ActionType = actionType;
        TargetType = targetType;
        RelatedEntityType = relatedEntityType;
        TargetID = targetID;
        RelatedEntityID = relatedEntityID;
        LocalChangeJSON = localChangeJSON;
        CreatedAt = DateTime.UtcNow;
        ProcessedAt = null;
        Status = UserActionStatus.Processing;
    }

    private UserAction()
    {
        
    }

    public void Approve()
    {
        if (Status != UserActionStatus.Processing)
        {
            return;
        }

        Status = UserActionStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != UserActionStatus.Processing)
        {
            return;
        }

        Status = UserActionStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }
}

using Infrastructure.Enums;
using Workflow.Enums;

namespace Infrastructure.Entities;

public class RequestedAction
{
    public int ID {get; private set;}
    public int UserID {get; private set;}
    public ActionType ActionType {get; private set;}
    public DomainEntityType TargetType {get; private set;}
    public DomainEntityType? RelatedEntityType {get; private set;}
    public int TargetID {get; private set;}
    public int? RelatedEntityID {get; private set;}
    public string? ChangeJSON {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}
    public DateTime? ProcessedAt {get; private set;}
    public string Justification {get; set;} = null!;
    public RequestedActionStatus Status {get; private set;}

    public RequestedAction(int userID, ActionType actionType, DomainEntityType targetType, DomainEntityType? relatedEntityType, int targetID, int? relatedEntityID, string? changeJSON, string justification)
    {
        UserID = userID;
        ActionType = actionType;
        TargetType = targetType;
        RelatedEntityType = relatedEntityType;
        TargetID = targetID;
        RelatedEntityID = relatedEntityID;
        ChangeJSON = changeJSON;
        CreatedAt = DateTime.UtcNow;
        ProcessedAt = null;
        Justification = justification;
        Status = RequestedActionStatus.Pending;
    }

    private RequestedAction()
    {
        
    }

    public void Approve()
    {
        if (Status == RequestedActionStatus.Approved)
        {
            return;
        }

        Status = RequestedActionStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status == RequestedActionStatus.Rejected)
        {
            return;
        }

        Status = RequestedActionStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }
}

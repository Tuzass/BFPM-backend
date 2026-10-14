using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class Notification
{
    public int ID {get; private set;}
    public DomainEntityType EntityType {get; private set;}
    public int EntityID {get; private set;}
    public string Content {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}
    public DateTime? SentAt {get; private set;}
    public NotificationStatus Status {get; private set;}

    private Notification()
    {
        
    }

    public Notification(DomainEntityType entityType, int entityID, string content)
    {
        EntityType = entityType;
        EntityID = entityID;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        SentAt = null;
        Status = NotificationStatus.Pending;
    }

    public void Send()
    {
        if (Status == NotificationStatus.Sent)
        {
            return;
        }

        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }
}

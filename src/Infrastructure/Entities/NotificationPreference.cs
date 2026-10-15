using Workflow.Enums;

namespace Infrastructure.Entities;

public class NotificationPreference
{
    public int UserID {get; private set;}
    public int ProjectID {get; private set;}
    public Trigger Trigger {get; private set;}
    public bool Enabled {get; set;}
    public bool SendEmail {get; set;}

    public NotificationPreference(int userID, int projectID, Trigger trigger)
    {
        UserID = userID;
        ProjectID = projectID;
        Trigger = trigger;
        Enabled = true;
        SendEmail = true;
    }

    private NotificationPreference()
    {
        
    }
}

namespace Infrastructure.Entities;

public class ProjectReservation
{
    public int ProjectID {get; private set;}
    public int UserActionID {get; private set;}

    public ProjectReservation(int projectID, int userActionID)
    {
        ProjectID = projectID;
        UserActionID = userActionID;
    }

    private ProjectReservation()
    {
        
    }
}

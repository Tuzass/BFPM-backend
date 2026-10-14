using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class SystemActionPreference
{
    public int UserID {get; private set;}
    public int ProjectID {get; private set;}
    public ProjectRole Role {get; set;}

    public SystemActionPreference(int userID, int projectID, ProjectRole role)
    {
        UserID = userID;
        ProjectID = projectID;
        Role = role;
    }

    private SystemActionPreference()
    {
        
    }
}

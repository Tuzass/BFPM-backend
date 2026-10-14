using Domain.Enums;

namespace Domain.Entities;

public class Project
{
    public int ID {get; private set;}
    public int OwnerID {get; private set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public ProjectStatus Status {get; private set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? CompletedAt {get; private set;}
    public int Version {get; private set;}

    private Project()
    {
        
    }

    public Project(int id, int ownerID, string name, string description, ProjectStatus status, DateTime? createdAt, DateTime? completedAt, int? version = null)
    {
        ID = id;
        OwnerID = ownerID;
        Name = name;
        Description = description;
        Status = status;
        CreatedAt = (createdAt == null) ? DateTime.UtcNow : (DateTime)createdAt;
        CompletedAt = completedAt;

        if (version is null)
        {
            Version = 1;
        }

        else
        {
            Version = version.Value;
        }
    }

    public void IncreaseVersion()
    {
        Version++;
    }

    public bool ChangeStatus(ProjectStatus newStatus)
    {
        if (newStatus == ProjectStatus.Completed && Status != ProjectStatus.Completed)
        {
            return false;
        }

        if (Status == ProjectStatus.Completed)
        {
            CompletedAt = null;
        }

        Status = newStatus;
        return false;
    }

    public void Complete()
    {
        if (Status == ProjectStatus.Completed)
        {
            return;
        }

        Status = ProjectStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}

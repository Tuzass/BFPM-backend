using Domain.Enums;
using Infrastructure.Entities;

namespace API.Responses;

public class ProjectResponse
{
    public int ID {get; set;}
    public string Name {get; set;} = null!;
    public string Description {get; set;} = null!;
    public int OwnerID {get; set;}
    public string Owner {get; set;} = null!;
    public ProjectStatus Status {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime? CompletedAt {get; set;}
    public int Version {get; set;}

    public ProjectResponse(int id, string name, string description, int ownerID, string owner, ProjectStatus status, DateTime createdAt, DateTime? completedAt, int version)
    {
        ID = id;
        Name = name;
        Description = description;
        OwnerID = ownerID;
        Owner = owner;
        Status = status;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        Version = version;
    }

    private ProjectResponse()
    {
        
    }
}

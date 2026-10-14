namespace Infrastructure.Entities;

public class TaskAssignment
{
    public int UserID {get; private set;}
    public int TaskID {get; private set;}

    public TaskAssignment(int userID, int taskID)
    {
        UserID = userID;
        TaskID = taskID;
    }

    private TaskAssignment()
    {
        
    }
}

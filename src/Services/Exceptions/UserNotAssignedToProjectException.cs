namespace Services.Exceptions;

public class UserNotAssignedToProjectException : Exception
{
    public UserNotAssignedToProjectException()
        : base("User is not assigned to project.")
    {
    }
}

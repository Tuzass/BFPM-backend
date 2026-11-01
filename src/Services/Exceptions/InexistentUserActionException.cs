namespace Services.Exceptions;

public class InexistentUserActionException : Exception
{
    public InexistentUserActionException()
        : base("The requested user action doesn't exist.")
    {
    }
}

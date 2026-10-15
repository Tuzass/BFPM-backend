namespace Services.Exceptions;

public class InexistentUserException : Exception
{
    public InexistentUserException()
        : base("User does not exist.")
    {
    }
}

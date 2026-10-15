namespace Services.Exceptions;

public class DuplicateUsernameException : Exception
{
    public DuplicateUsernameException()
        : base("A user with this username already exists.")
    {
    }
}

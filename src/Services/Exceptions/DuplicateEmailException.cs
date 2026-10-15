namespace Services.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException()
        : base("This email already belongs to a user.")
    {
    }
}

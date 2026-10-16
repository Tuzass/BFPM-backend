namespace Services.Exceptions;

public class InexistentSystemActionPreferenceException : Exception
{
    public InexistentSystemActionPreferenceException()
        : base("The project has no such system preference.")
    {
    }
}

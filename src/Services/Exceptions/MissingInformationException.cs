namespace Services.Exceptions;

public class MissingInformationException : Exception
{
    public MissingInformationException()
        : base("Couldn't create entity due to missing information.")
    {
    }
}

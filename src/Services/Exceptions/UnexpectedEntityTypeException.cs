namespace Services.Exceptions;

public class UnexpectedEntityTypeException : Exception
{
    public UnexpectedEntityTypeException()
        : base("Unexpected entity type received.")
    {
    }
}

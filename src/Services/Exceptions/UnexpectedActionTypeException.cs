namespace Services.Exceptions;

public class UnexpectedActionTypeException : Exception
{
    public UnexpectedActionTypeException()
        : base("Unexpected action type received.")
    {
    }
}

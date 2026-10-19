namespace Services.Exceptions;

public class UnexpectedActionTargetException : Exception
{
    public UnexpectedActionTargetException()
        : base("Unexpected action target received.")
    {
    }
}

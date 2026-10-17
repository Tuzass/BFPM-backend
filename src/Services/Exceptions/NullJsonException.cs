namespace Services.Exceptions;

public class NullJsonException : Exception
{
    public NullJsonException()
        : base("JSON to string conversion resulted in null.")
    {
    }
}

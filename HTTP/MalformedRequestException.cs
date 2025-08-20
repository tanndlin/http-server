namespace HTTP;

internal class MalformedRequestException : Exception
{
    public MalformedRequestException(string? message) : base(message)
    {
    }
}

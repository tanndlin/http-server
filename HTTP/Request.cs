namespace HTTP;

internal enum RequestMethod
{
    GET,
}

internal class Request
{
    public readonly RequestMethod method;

    public Request(RequestMethod method)
    {
        this.method = method;
    }
}

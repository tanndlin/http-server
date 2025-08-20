using System.Text;
using Util;

namespace HTTP;

public class HTTPConnection : IByteHandler
{
    public HTTPConnection()
    {
        Console.WriteLine("HTTP Connection created");
    }

    public byte[] HandleRequest(byte[] bytes)
    {
        Console.WriteLine("Got data");
        Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
        Console.WriteLine();

        Request req = RequestParser.Parse(bytes);
        return req.method switch
        {
            RequestMethod.GET => HandleGetRequest((GetRequest)req),
            _ => throw new NotSupportedException($"HTTP method {req.method} is not supported")
        };
    }

    private byte[] HandleGetRequest(GetRequest getRequest)
    {
        Console.WriteLine("Handling GET request");

        string page = "Hello World!";
        byte[] content = Encoding.UTF8.GetBytes(page);
        int contentLength = content.Length;

        StringBuilder resBuilder = new("HTTP/1.1 200 OK\r\n");
        resBuilder.Append("Content-Type: text/plain\r\n");
        resBuilder.Append($"Content-Length: {contentLength}\r\n");
        resBuilder.Append("Connection: keep-alive\r\n");
        resBuilder.Append("\r\n");
        return [.. Encoding.UTF8.GetBytes(resBuilder.ToString()).Concat(content)];
    }

    public byte[] HandleBytes(byte[] bytes) => HandleRequest(bytes);
}

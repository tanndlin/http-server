using Util;

namespace HTTP;

/// <summary>
/// HTTP is a stateless protocol so all methods will be static
/// </summary>
public class HTTPConnection : IByteHandler
{
    public static byte[] HandleRequest(byte[] bytes)
    {
        Console.WriteLine("Got data");
        Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
        Console.WriteLine();

        try
        {
            Request req = RequestParser.Parse(bytes);
            return req.method switch
            {
                RequestMethod.GET => HandleGetRequest((GetRequest)req),
                _ => throw new NotSupportedException($"HTTP method {req.method} is not supported")
            };
        }
        catch (Exception e)
        {
            return ResponseBuilder.BuildResponse(400, [], e.Message);
        }
    }

    private static byte[] HandleGetRequest(GetRequest getRequest)
    {
        Console.WriteLine("Handling GET request");

        string requestPath = getRequest.path.TrimStart('/');
        // Check if the path has an extension
        if (!Path.HasExtension(requestPath))
            requestPath += ".html"; // Default to .html if no extension is provided


        string path = Path.Combine(Directory.GetCurrentDirectory(), requestPath);
        if (!File.Exists(path))
        {
            return ResponseBuilder.BuildResponse(404, new Dictionary<string, string>
            {
                { "Content-Type", "text/plain" },
                { "Connection", "close" }
            }, "404 Not Found");
        }

        byte[] content = File.ReadAllBytes(path);
        return ResponseBuilder.BuildResponse(200, new Dictionary<string, string>
            {
                { "Content-Type", ResponseBuilder.GetContentType(path) },
                { "Connection", "keep-alive" }
            }, content);
    }

    public byte[] HandleBytes(byte[] bytes) => HandleRequest(bytes);
}

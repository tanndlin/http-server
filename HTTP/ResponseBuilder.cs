using System.Text;

namespace HTTP;
internal static class ResponseBuilder
{
    public static byte[] BuildResponse(int statusCode, string statusMessage, Dictionary<string, string> headers, byte[] body)
    {
        StringBuilder response = new($"HTTP/1.1 {statusCode} {statusMessage}\r\n");

        // Headers
        foreach (var header in headers)
            response.Append($"{header.Key}: {header.Value}\r\n");

        if (!headers.ContainsKey("Content-Length"))
            response.Append($"Content-Length: {body.Length}\r\n");

        response.Append("\r\n");

        byte[] headerBytes = Encoding.UTF8.GetBytes(response.ToString());
        return [.. headerBytes.Concat(body)];
    }

    public static byte[] BuildResponse(int statusCode, string statusMessage, Dictionary<string, string> headers, string body)
        => BuildResponse(statusCode, statusMessage, headers, Encoding.UTF8.GetBytes(body));

    public static byte[] BuildResponse(int statusCode, Dictionary<string, string> headers, byte[] body)
        => BuildResponse(statusCode, GetStatusMessage(statusCode), headers, body);

    public static byte[] BuildResponse(int statusCode, Dictionary<string, string> headers, string body)
        => BuildResponse(statusCode, GetStatusMessage(statusCode), headers, Encoding.UTF8.GetBytes(body));

    private static string GetStatusMessage(int statusCode) => statusCode switch
    {
        200 => "OK",
        400 => "Bad Request",
        404 => "Not Found",
        500 => "Internal Server Error",
        _ => "Unknown Status"
    };

    public static string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            _ => throw new MalformedRequestException($"Unexpected file type in GET Request (ext: {Path.GetExtension(filePath).ToLower()})")
        };
    }
}

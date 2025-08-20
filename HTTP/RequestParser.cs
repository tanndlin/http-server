namespace HTTP;

internal static class RequestParser
{
    public static Request Parse(byte[] bytes)
    {
        string data = System.Text.Encoding.UTF8.GetString(bytes);
        string[] lines = data.Split("\n");

        if (!Enum.TryParse(lines[0].Split(" ")[0], out RequestMethod method))
        {
            throw new InvalidDataException("Unknown HTTP request method");
        }

        return method switch
        {
            RequestMethod.GET =>
                ParseGetRequest(lines),
            _ => throw new NotSupportedException($"HTTP method {method} is not supported")
        };
    }

    private static Request ParseGetRequest(string[] lines)
    {
        string[] requestLine = lines[0].Split(" ");
        if (requestLine.Length != 3)
        {
            throw new InvalidDataException("Invalid GET request format");
        }

        string path = requestLine[1];
        string httpVersion = requestLine[2];
        Dictionary<string, string> queryParams = new();

        return new GetRequest(path, queryParams);
    }
}

namespace HTTP;

internal static class RequestParser
{
    public static Request Parse(byte[] bytes)
    {
        string data = System.Text.Encoding.UTF8.GetString(bytes);
        string header = data.Split("\r\n\r\n").First();
        string[] lines = header.Split("\r\n");
        if (lines.Length == 1)
            throw new MalformedRequestException("Malformed request");

        if (!Enum.TryParse(lines[0].Split(" ")[0], out RequestMethod method))
            throw new NotImplementedException($"Unknown HTTP request method ({lines[0].Split(" ")[0]})");


        return method switch
        {
            RequestMethod.GET => ParseGetRequest(lines),
            _ => throw new NotImplementedException(), // Not possible
        };
    }

    private static GetRequest ParseGetRequest(string[] lines)
    {
        string[] methodLine = lines[0].Split(" ");
        if (methodLine.Length != 3)
        {
            throw new MalformedRequestException("Invalid GET request format");
        }

        string path = methodLine[1];
        string httpVersion = methodLine[2];
        Dictionary<string, string> queryParams = new();
        foreach (string line in lines.Skip(1))
        {
            var split = line.Split(": ");
            if (split.Length <= 1)
                throw new MalformedRequestException($"Header was malformed (header: {line})");

            queryParams[split[0]] = split[1];
        }

        return new GetRequest(path, queryParams);
    }
}

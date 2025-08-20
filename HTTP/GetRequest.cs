using System.Text;

namespace HTTP;
internal class GetRequest : Request
{
    public readonly string path;
    public readonly Dictionary<string, string> queryParams;
    public GetRequest(string path, Dictionary<string, string> queryParams) : base(RequestMethod.GET)
    {
        this.path = path;
        this.queryParams = queryParams;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"GET Request to {path}");
        if (queryParams.Count > 0)
        {
            sb.AppendLine("Query Parameters:");
            foreach (var param in queryParams)
            {
                sb.AppendLine($"{param.Key}: {param.Value}");
            }
        }
        return sb.ToString();
    }
}

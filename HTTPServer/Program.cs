using System.Net;
using HTTPServer;

Server server = new(new IPEndPoint(IPAddress.Any, 6969), data =>
{
    // Example callback that echoes back the received data
    return data;
});

server.Start();
Console.WriteLine("Server started. Press Enter to stop...");
Console.ReadLine();
server.Dispose();
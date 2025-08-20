using System.Net;
using HTTP;

Server.Server<HTTPConnection> server = new(new IPEndPoint(IPAddress.Any, 6969));

server.Start();
Console.WriteLine("Server started. Press Enter to stop...");
Console.ReadLine();
server.Dispose();
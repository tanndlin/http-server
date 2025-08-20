using System.Net.Sockets;

namespace HTTPServer;

internal struct Client
{
    public readonly TcpClient client;
    public readonly Thread listenThread;

    public Client(TcpClient client, Thread listenThread)
    {
        this.client = client;
        this.listenThread = listenThread;
    }
}

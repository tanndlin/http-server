using System.Net;
using System.Net.Sockets;

namespace HTTPServer;

internal class Server : IDisposable
{
    private readonly IPEndPoint endpoint;

    private readonly TcpListener listener;
    private readonly List<Client> clients = [];

    private Thread runThread;
    private readonly Func<byte[], byte[]> callback;

    private bool disposedValue;

    public Server(IPEndPoint ep, Func<byte[], byte[]> callback)
    {
        endpoint = ep;
        listener = new TcpListener(endpoint);
    }

    public void Start()
    {
        listener.Start();
        runThread = new(() =>
        {
            while (listener.Server.IsBound)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Thread thread = new Thread(() => AcceptClient(client));
                    clients.Add(new(client, thread));
                }
                catch (SocketException ex)
                {
                    // Listener was stopped, exit the loop
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        });

        runThread.Start();
    }

    private void AcceptClient(TcpClient client)
    {
        var stream = client.GetStream();
        byte[] buffer = new byte[2048];
        while (true)
        {
            if (client.Available > 0)
            {
                int bytesRead = stream.Read(buffer);
                byte[] data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);

                var response = callback(data);
                if (response.Length > 0)
                {
                    stream.Write(response, 0, response.Length);
                }
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                listener.Stop();
                if (runThread != null && runThread.IsAlive)
                {
                    runThread.Join();
                }
                listener.Dispose();

                clients.ForEach(c => c.client.Close());
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

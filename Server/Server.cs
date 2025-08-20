using System.Net;
using System.Net.Sockets;
using Util;

namespace Server;

public class Server<T> : IDisposable where T : IByteHandler, new()
{
    private readonly IPEndPoint endpoint;

    private readonly TcpListener listener;
    private readonly List<Client> clients = [];

    private readonly Thread runThread;

    private bool disposedValue;

    public Server(IPEndPoint ep)
    {
        endpoint = ep;
        listener = new TcpListener(endpoint);

        runThread = new(() =>
        {
            while (listener.Server.IsBound)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    T handler = new();
                    Thread thread = new(() => AcceptClient(client, handler.HandleBytes));
                    thread.Start();
                    clients.Add(new(client, thread));
                }
                catch (SocketException)
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
    }

    public void Start()
    {
        listener.Start();
        runThread.Start();
        Console.WriteLine($"Server started on: {listener.LocalEndpoint}");
    }

    private void AcceptClient(TcpClient client, Func<byte[], byte[]> callback)
    {
        Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

        var stream = client.GetStream();
        byte[] buffer = new byte[2048];
        while (client.Connected)
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
                    stream.Flush();
                }
            }

            Thread.Sleep(1);
        }

        Console.WriteLine($"Client Disconnected");
        clients.Remove(clients.First(c => c.client == client));
        client.Dispose();
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
                clients.Clear();
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

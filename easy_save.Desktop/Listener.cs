using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace easy_save.Desktop
{
    public class Listener : IDisposable
    {
        private bool closed = false;
        private readonly Socket listenSocket;
        private readonly List<Client> clients = new List<Client>();

        public Listener()
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening(int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listenSocket.Bind(endPoint);
            listenSocket.Listen(10);


            Thread thread = new Thread(WaitForClient);
            thread.Start();
        }

        private void WaitForClient()
        {
            while (!closed)
            {
                try
                {
                    Socket clientSocket = listenSocket.Accept();
                    Client client = new Client(clientSocket);
                    clients.Add(client);

                    client.Disposed += ((sender, e) =>
                    {
                        clients.Remove(sender as Client);
                    });

                    client.Start();
                }
                catch { break; }
            }
        }


        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    listenSocket.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            while (clients.Count > 0)
                clients[0].Dispose();
            closed = true;

            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace easy_save.Desktop
{
    public class Client : IDisposable
    {
        private bool closed;
        private readonly Socket socket;

        public event EventHandler Disposed;

        public Client(Socket socket)
        {
            this.socket = socket;
        }

        public void Start()
        {
            Thread thread = new Thread(Listen);
            thread.Start();
        }

        private void Listen()
        {
            byte[] buffer = new byte[1024];

            while (!closed)
            {
                try
                {
                    int count = socket.Receive(buffer);
                    //socket.Send(Serializer.Serialize("Message bien reçu"));
                    if (count == 0)
                        throw new SocketException();
                }
                catch (SocketException)
                {
                    Dispose();
                }
            }
        }


        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    socket.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            if (closed)
                return;
            closed = true;
            Disposed?.Invoke(this, EventArgs.Empty);

            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
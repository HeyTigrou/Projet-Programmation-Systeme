using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace easy_save.Lib.SocketListener
{
    public class Client : IDisposable
    {
        private bool closed;
        private readonly Socket socket;

        public event EventHandler Disposed;
        public event EventHandler<string> MessageReceived;

        public Client(Socket socket)
        {
            this.socket = socket;
        }

        public void Start()
        {
            Thread thread = new Thread(Listen);
            thread.Start();
        }

        /// <summary>
        /// Gets the messages and invoke an event to decode it.
        /// </summary>
        private void Listen()
        {
            byte[] buffer = new byte[1024];

            while (!closed)
            {
                try
                {
                    int count = socket.Receive(buffer);
                    if (count == 0)
                        throw new SocketException();
                    string message = Encoding.UTF8.GetString(buffer, 0, count);

                    MessageReceived?.Invoke(this, message);
                }
                catch (SocketException)
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// Sends message to client.
        /// </summary>
        /// <param name="message"></param>
        public void send(string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            socket.Send(buffer);
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
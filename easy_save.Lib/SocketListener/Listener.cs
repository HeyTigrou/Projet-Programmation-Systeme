using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using easy_save.Lib.Models;
using System.Diagnostics;
using System.Xml.Linq;
using System.Configuration;

namespace easy_save.Lib.SocketListener
{
    public class Listener : IDisposable
    {
        private bool closed = false;
        private readonly Socket listenSocket;
        private readonly List<Client> clients = new List<Client>();


        public event EventHandler RefreshCommandReceived;
        public event EventHandler<string> StopCommandReceived;
        public event EventHandler<string> PauseCommandReceived;
        public event EventHandler<string> ResumeCommandReceived;
        public event EventHandler<string> RemoveCommandReceived;
        public event EventHandler<string> LaunchSaveCommandReceived;

        public Listener()
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Waits for client connections, up to 10 connections.
        /// </summary>
        /// <param name="port"></param>
        public void StartListening(int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ServerIp"]), port);
            listenSocket.Bind(endPoint);
            listenSocket.Listen(10);


            Thread thread = new Thread(WaitForClient);
            thread.Start();
        }

        /// <summary>
        /// Adds clients
        /// </summary>
        private void WaitForClient()
        {
            while (!closed)
            {
                Socket clientSocket = listenSocket.Accept();
                Client client = new Client(clientSocket);
                clients.Add(client);

                client.Disposed += ((sender, e) =>
                {
                    clients.Remove(sender as Client);
                });
                client.MessageReceived += Message_Received;

                client.Start();
            }
        }

        /// <summary>
        /// Sends the state of the save work to clients.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        public void SendState(string name, string state)
        {
            string message = $"State||{name}||{state}||";
            foreach (var client in clients)
            {
                client.send(message);
            }
        }

        /// <summary>
        /// Send the progression fo the save work to clients.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="progression"></param>
        public void SendProgression(string name, string progression)
        {
            string message = $"Progression||{name}||{progression}||";
            foreach (var client in clients)
            {
                client.send(message);
            }
        }

        /// <summary>
        /// Send refresh command to clients.
        /// </summary>
        public void SendRefresh()
        {
            string message = "Refresh||";
            foreach (var client in clients)
            {
                client.send(message);
            }
        }

        /// <summary>
        /// Sends save work to client to show in the clients grid.
        /// </summary>
        /// <param name="saveWork"></param>
        public void SendSaveWork(SaveWorkModel saveWork)
        {
            string message = $"SaveWork||{saveWork.Name}||{saveWork.InputPath}||{saveWork.OutputPath}||{saveWork.SaveType}||{saveWork.Progression}||{saveWork.State}||";
            foreach (var client in clients)
            {
                client.send(message);
            }
        }

        /// <summary>
        /// Parses the received message, to know which function to launch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void Message_Received(Object sender, string message)
        {
            string[] splitedMessage = message.Split("||");
            switch (splitedMessage[0])
            {
                case "Stop":
                    {
                        StopCommandReceived?.Invoke(this, splitedMessage[1]);
                        break;
                    }
                case "Pause":
                    {
                        PauseCommandReceived?.Invoke(this, splitedMessage[1]);
                        break;
                    }
                case "Resume":
                    {
                        ResumeCommandReceived?.Invoke(this, splitedMessage[1]);
                        break;
                    }
                case "Refresh":
                    {
                        RefreshCommandReceived?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                case "Remove":
                    {
                        RemoveCommandReceived?.Invoke(this, splitedMessage[1]);
                        break;
                    }
                case "LaunchSave":
                    {
                        LaunchSaveCommandReceived?.Invoke(this, splitedMessage[1]);
                        break;
                    }
                default: break;
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

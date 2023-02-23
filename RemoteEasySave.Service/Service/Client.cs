using System.Net.Sockets;
using System.Net;
using System.Threading;
using RemoteEasySave.Lib.Models;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using System.Configuration;

namespace RemoteEasySave.Lib.Service
{
    public class Client
    {
        private Socket ConnexionSocket;
        public int Port;
        public event EventHandler<SaveWorkModel> AddSaveWork;
        public event EventHandler ClearSaveWorkCollection;
        public bool closed = false;

        public ObservableCollection<SaveWorkModel> Processes;
        public Client(ObservableCollection<SaveWorkModel> processes)
        {
            Port = Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            Processes = processes;
            ConnexionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
        }

        public void Start()
        {
            bool connected = false;
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ServerIp"]), Port);
            while (!connected)
            {
                try
                {

                    ConnexionSocket.Connect(server);
                    connected= true;
                    
                }
                catch { connected = false; Thread.Sleep(100); }
            }

            new Thread(_ =>
            {
                while (true)
                {
                    try
                    {
                        byte[] buffer1 = new byte[1024];
                        int Length = ConnexionSocket.Receive(buffer1);
                        if (Length == 0)
                            throw new SocketException();
                        string message = System.Text.Encoding.UTF8.GetString(buffer1, 0, Length);
                        Decode(message);
                    }
                    catch(SocketException)
                    {
                        Dispose();
                        break;
                    }
                }
            }).Start();
        }


        public void Send(string message)
        {
            try
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
                ConnexionSocket.Send(buffer);
            }
            catch (SocketException)
            {
                Dispose();
            }
        }

        public void Decode(string message)
        {
            string[] splitedMessage = message.Split("||");
            switch (splitedMessage[0])
            {
                case "SaveWork":
                    {
                        List<string> list = splitedMessage.Cast<string>().ToList();
                        while (list.Count >= 7)
                        {
                            SaveWorkModel saveWorkModel = new SaveWorkModel()
                            {
                                Name = list[1],
                                InputPath = list[2],
                                OutputPath = list[3],
                                SaveType = int.Parse(list[4]),
                                Progression = list[5],
                                State = list[6]
                            };
                            list.RemoveRange(1, 7);
                            AddSaveWork?.Invoke(this, saveWorkModel);
                        }
                        break;

                    }
                case "Progression":
                    {
                        string name = splitedMessage[1];
                        string progression = splitedMessage[2];
                        foreach (SaveWorkModel saveWork in Processes)
                        {
                            if (saveWork.Name == name)
                            {
                                saveWork.Progression = progression;
                            }
                        }
                        break;
                    }
                case "Refresh":
                    ClearSaveWorkCollection?.Invoke(this, EventArgs.Empty);
                    break;
                case "State":
                    {
                        string name = splitedMessage[1];
                        string state = splitedMessage[2];
                        foreach (SaveWorkModel saveWork in Processes)
                        {
                            if (saveWork.Name == name)
                            {
                                saveWork.State = state;
                            }
                        }
                        break;
                    }
                default: break;
            }
        }
        #region IDISPOSABLE

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ConnexionSocket.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            if (closed)
                return;
            closed = true;

            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDISPOSABLE
    }
}
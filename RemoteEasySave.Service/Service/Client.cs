using System.Net.Sockets;
using System.Net;
using System.Threading;
using RemoteEasySave.Lib.Models;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;

namespace RemoteEasySave.Lib.Service
{
    public class Client
    {
        private Socket ConnexionSocket;
        public int Port;
        public ObservableCollection<SaveWorkModel> Processes;
        public Client(int port, ObservableCollection<SaveWorkModel> processes)
        {
            Port = port;
            Processes = processes;
            ConnexionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            ConnexionSocket.Connect(server);

            new Thread(_ =>
            {
                while (true)
                {
                    byte[] buffer1 = new byte[1024];
                    int Length = ConnexionSocket.Receive(buffer1);
                    string message = System.Text.Encoding.UTF8.GetString(buffer1, 0, Length);
                    Decode(message);
                }
            }).Start();
        }


        public void Send(string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            ConnexionSocket.Send(buffer);
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
                                State = list[5],
                                Progression = list[6],
                            };
                            list.RemoveRange(1, 7);
                            Processes.Add(saveWorkModel);
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
                    Processes.Clear();
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
    }
}
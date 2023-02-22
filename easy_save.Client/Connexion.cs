using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace easy_save.Client
{
    internal class Connexion
    {
        public static void Main(string[] args)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                IPEndPoint server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55894);
                socket.Connect(server);

                Console.WriteLine($"Client connected to {server.Address}:{server.Port}");

                // Message de bienvenue
                byte[] buffer = new byte[1024];
                int count = socket.Receive(buffer);
                Console.WriteLine(Serializer.Deserialize(buffer, count));

                Console.WriteLine("Tap quit to exit");

                do
                {
                    string message = Console.ReadLine();
                    if (message == "quit")
                        break;

                    try { socket.Send(Serializer.Serialize(message)); }
                    catch { break; }
                } while (true);

                Console.WriteLine("Server disconnected");
            }
        }
    }
}
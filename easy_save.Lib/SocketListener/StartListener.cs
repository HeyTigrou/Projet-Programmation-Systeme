using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.SocketListener
{
    public static class SocketConnection
    {
        public static bool connected = false;
        public static Listener server = new Listener();
        public static void Connect(int port)
        {
            server.StartListening(port);
        }
    }
}

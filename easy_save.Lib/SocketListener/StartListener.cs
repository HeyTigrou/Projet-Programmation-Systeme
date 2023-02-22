using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.SocketListener
{
    public class SocketConnection
    {
        public Listener server = new Listener();
        public void Connect(int port)
        {
            server.StartListening(port);
        }
    }
}

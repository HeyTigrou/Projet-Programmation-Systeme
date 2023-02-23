using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.SocketListener
{
    public class SocketConnection
    {
        public Listener server = new Listener();
        public void Connect()
        {
            int port = Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            server.StartListening(port);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Desktop
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Listener server = new Listener();
            server.StartListening(42042);
        }
    }
}

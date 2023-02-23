using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RemoteEasySave.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, "RemoteEasySave", out bool createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Error RemoteEasySave is already running");
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}

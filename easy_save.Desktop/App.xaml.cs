using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace easy_save.Desktop
{
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "easy_save.Desktop";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Error you can't launch two instances of EasySave");
                Application.Current.Shutdown(); 
            }

            base.OnStartup(e);
        }
    }
}

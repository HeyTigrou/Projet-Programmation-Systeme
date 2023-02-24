using System.Threading;
using System.Windows;

namespace easy_save.Desktop
{
    public partial class App : Application
    {
        private static Mutex mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "EasySave", out bool createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Error EasySave is already running");
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}
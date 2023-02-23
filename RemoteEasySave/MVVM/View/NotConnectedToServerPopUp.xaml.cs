using System.Windows;

namespace RemoteEasySave.Desktop.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour PopupProcessRunningView.xaml
    /// </summary>
    public partial class NotConnectedToServerPopUp : Window
    {
        public NotConnectedToServerPopUp()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

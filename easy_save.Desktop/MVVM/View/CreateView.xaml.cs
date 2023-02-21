using System.Windows;
using System.Windows.Forms;

namespace easy_save.Desktop.MVVM.View
{
    public partial class CreateView : System.Windows.Controls.UserControl
    {
        public CreateView()
        {
            InitializeComponent();
        }

        public void btnInputPath(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedDirectory = dialog.SelectedPath;
                sourceTextBox.Text = selectedDirectory;
            }
        }

        public void btnOutputPath(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedDirectory = dialog.SelectedPath;
                destinationTextBox.Text = selectedDirectory;
            }
        }
    }
}
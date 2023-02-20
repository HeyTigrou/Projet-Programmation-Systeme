using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace easy_save.Desktop
{
    public partial class MainWindow : Window
    {
        private static Mutex mutex = new Mutex(false);
        public MainWindow()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {

                InitializeComponent();
                SwitchLanguage("en");
            }
            else {

                Environment.Exit(0);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FrenchButton_Checked(object sender, RoutedEventArgs e)
        {
            SwitchLanguage("fr");
        }

        private void EnglishButton_Checked(object sender, RoutedEventArgs e)
        {
            SwitchLanguage("en");
        }

        private void SwitchLanguage(string languageCode)
        {
            ResourceDictionary dict = new ResourceDictionary
            {
                Source = languageCode switch
                {
                    "en" => new Uri(@"..\Languages\LanguageEn.xaml", UriKind.Relative),
                    "fr" => new Uri(@"..\Languages\LanguageFr.xaml", UriKind.Relative),
                    _ => new Uri(@"..\Languages\StringResources_en.xaml", UriKind.Relative),
                }
            };
            Resources.MergedDictionaries.Add(dict);
        }

    }
}

using System;
using System.Threading;
using System.Windows;

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
            else
            {
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
                    "fr" => new Uri(@"..\Languages\LanguageFr.xaml", UriKind.Relative)
                }
            };
            Resources.MergedDictionaries.Add(dict);
        }
    }
}
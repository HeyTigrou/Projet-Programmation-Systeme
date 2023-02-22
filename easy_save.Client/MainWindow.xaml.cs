using System;
using System.Windows;

namespace easy_save.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SwitchLanguage("en");
            Connexion.Start();
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
                }
            };
            Resources.MergedDictionaries.Add(dict);
        }

    }
}
using easy_save.Cmd.Views;

namespace easy_save.Cmd
{
    internal class Program
    {
        static void Main(string[] args) // Launch of the program
        {
            MainMenuView view = MainMenuView.Instance; // Recuperation of the unique instance of the console interface
            view.Display(); // Launch of the console interface
        }
    }
}
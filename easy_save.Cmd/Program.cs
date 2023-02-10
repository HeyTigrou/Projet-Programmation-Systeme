using easy_save.Cmd.Views;

namespace easy_save.Cmd
{
    internal class Program
    {
        // Launch of the program
        static void Main(string[] args) 
        {
            // Recuperation of the unique instance of the console interface
            MainMenuView view = new MainMenuView();
            // Launch of the console interface
            view.Display(); 
        }
    }
}
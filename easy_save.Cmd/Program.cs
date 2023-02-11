using easy_save.Cmd.Views;

namespace easy_save.Cmd
{
    internal class Program
    {
        // Launch of the program
        static void Main(string[] args) 
        {
            MainMenuView view = new MainMenuView();
            view.Display(); 
        }
    }
}
using easy_save.Cmd.Views;

namespace easy_save.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainMenuView view = MainMenuView.Instance;
            view.Display();
        }
    }
}
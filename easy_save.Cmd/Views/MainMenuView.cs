using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using easy_save.Lib.ViewModels;

namespace easy_save.Cmd.Views
{
    internal class MainMenuView
    {
        private readonly FileSaveViewModel fileSaveViewModel = new();

        private static MainMenuView instance;
        
        public static MainMenuView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainMenuView();
                }
                return instance;
            }
        }
        
        private MainMenuView(){}
        
        public void Display()
        {
            while (true)
            {
                Console.WriteLine("Welcome to Easy Save");
                Console.WriteLine("1. Create a new backup");
                Console.WriteLine("2. Restore a backup");
                Console.WriteLine("3. View logs");
                Console.WriteLine("4. Exit");
                string inputChoice = Console.ReadLine();
                switch (inputChoice)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Enter the name of your save");
                        fileSaveViewModel.Name = Console.ReadLine();
                        Console.WriteLine("Enter the path of the file to save");
                        fileSaveViewModel.InputPath = Console.ReadLine();
                        Console.WriteLine("Enter the path of the destination");
                        fileSaveViewModel.OutputPath = Console.ReadLine();
                        Console.WriteLine("Enter the type of save(0 : All files / 1 : Changed files)");
                        fileSaveViewModel.SaveType = int.Parse(Console.ReadLine());
                        fileSaveViewModel.Save();
                        break;
                    case "2":
                        Console.Clear();
                        /*
                         Example use of the logger
                        var logger = new EasySaveLogger("logs", "C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\");
                        logger.logSaveFolderFilesProgression("Save1", "a", "b", "end", 0, 0, 0);
                        */
                        break;
                    case "3":
                        Console.Clear();
                        break;
                    case "4":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Please enter a valid choice");
                        break;
                }
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using easy_save.Lib.ViewModels;
using Newtonsoft.Json;

namespace easy_save.Cmd.Views
{
    internal class MainMenuView
    {
        private enum languages { en, fr }
        
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
                string Language = "";
                
                do
                {
                    Console.Clear();
                    Console.WriteLine("*** Welcome to EasySave ***\n ");
                    Console.WriteLine("Choose a language: \n> English => en \n> French => fr");
                    Console.Write("\nEnter an option: ");
                    Language = Console.ReadLine();
                }
                while (!(Enum.IsDefined(typeof(languages), Language)));

                dynamic languageJson = JsonConvert.DeserializeObject(File.ReadAllText($@"..\..\..\..\easy_save.Lib\ConfigurationFiles\Interface_text\{Language}.json"));

                bool inputIsOk;
                do
                {
                    inputIsOk = true;

                    Console.Clear();
                    Console.WriteLine("*** EasySave ***\n ");
                    Console.WriteLine(languageJson.Main.Menu);
                    Console.Write(languageJson.Main.Option);
                    string inputChoice = Console.ReadLine();


                    switch (inputChoice)
                    {
                        case "1":
                            
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
                            Console.WriteLine(languageJson.Save.Name);
                            fileSaveViewModel.Name = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.PathSave);
                            fileSaveViewModel.InputPath = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.PathDestination);
                            fileSaveViewModel.OutputPath = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.SaveType);
                            fileSaveViewModel.SaveType = int.Parse(Console.ReadLine());
                            fileSaveViewModel.Save();
                            // Add Proccess done
                            break;
                        case "4":
                            Environment.Exit(0);
                            break;
                        case "5":
                            foreach (var saveWork in FileSaveViewModel.GetAvailableWorks())
                            {
                                Console.WriteLine(saveWork);
                            }
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        case "6":
                            Console.Clear();
                            break;

                        default:
                            Console.Clear();
                            inputIsOk = false;
                            break;
                    }
                }
                while (!inputIsOk);
            } 
        }
    }
}

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
            while (true)
            {
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
                            Console.Clear();
                            Console.WriteLine(languageJson.Save.Name);
                            string name = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.PathSave);
                            string inputPath = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.PathDestination);
                            string outputPath = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.SaveType);
                            int saveType = int.Parse(Console.ReadLine());
                            bool success = FileSaveViewModel.CreateSaveWork(inputPath, outputPath, name, saveType);
                            if (success)
                            {
                                Console.WriteLine("=> Processus de sauvegarde crée");
                            }
                            else
                            {
                                Console.WriteLine("=> Nombre de processus de sauvegarde maximum atteint, impossible de créer");
                            }
                            Console.WriteLine("! Appuyer sur entrée pour continuer !");
                            Console.ReadLine();
                            break;
                            
                        case "2":
                            Console.Clear();
                            Console.WriteLine("Entrer le nom de la sauvegarde à supprimer");
                            string nameToDelete = Console.ReadLine();
                            bool successDelete = FileSaveViewModel.DeleteSaveWork(nameToDelete);
                            if (successDelete)
                            {
                                Console.WriteLine("=> Processus de sauvegarde supprimé");
                            }
                            else
                            {
                                Console.WriteLine("=> Processus de sauvegarde non trouvé");
                            }
                            Console.WriteLine("! Appuyer sur entrée pour continuer !");
                            Console.ReadLine();
                            break;
                            
                        case "3":
                            Console.Clear();
                            Console.WriteLine("Entrer le nom du processus de sauvegarde à lancer");
                            string saveName = Console.ReadLine();
                            bool successUse = FileSaveViewModel.Save(saveName);
                            if (successUse)
                            {
                                Console.WriteLine("=> Processus de sauvegarde lancé");
                            }
                            else
                            {
                                Console.WriteLine("=> Processus de sauvegarde non trouvé");
                            }
                            Console.WriteLine("! Appuyer sur entrée pour continuer !");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                            
                        case "4":
                            Console.Clear();
                            Console.WriteLine("Lancement de tout les processus de sauvegarde");
                            int errors = FileSaveViewModel.AllSave();
                            if (errors == 0)
                            {
                                Console.WriteLine("=> Tous les processus de sauvegarde ont été lancés corrêctement");
                            }
                            else
                            {
                                Console.WriteLine($"=> {errors} processus de sauvegarde n'a / ont pas pu(s) être lancé(s)");
                            }
                            Console.WriteLine("! Appuyer sur entrée pour continuer !");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                            
                        case "5":
                            foreach (var saveWork in FileSaveViewModel.GetAvailableWorks())
                            {
                                Console.WriteLine("=> " + saveWork);
                            }
                            Console.WriteLine("! Appuyer sur entrée pour continuer !");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                            
                        case "6":
                            Environment.Exit(0);
                            break;

                        default:
                            Console.Clear();
                            inputIsOk = false;
                            /*
                             Example use of the logger
                            var logger = new EasySaveLogger("logs", "C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\");
                            logger.logSaveFolderFilesProgression("Save1", "a", "b", "end", 0, 0, 0);
                            */
                            break;
                    }
                }
                while (!inputIsOk);
            } 
        }
    }
}

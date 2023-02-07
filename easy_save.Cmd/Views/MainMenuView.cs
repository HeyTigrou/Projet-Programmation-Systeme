using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
                                Console.WriteLine(languageJson.Save.Process);
                            }
                            else
                            {
                                Console.WriteLine(languageJson.Save.ErrorLimit);
                            }
                            Console.WriteLine(languageJson.Save.Continue);
                            Console.ReadLine();
                            break;
                            
                        case "2":
                            Console.Clear();
                            Console.WriteLine(languageJson.Remove.Name);
                            string nameToDelete = Console.ReadLine();
                            bool successDelete = FileSaveViewModel.DeleteSaveWork(nameToDelete);
                            if (successDelete)
                            {
                                Console.WriteLine(languageJson.Remove.SucessRemove);
                            }
                            else
                            {
                                Console.WriteLine(languageJson.Remove.MissingFile);
                            }
                            Console.WriteLine(languageJson.Remove.Continue);
                            Console.ReadLine();
                            break;
                            
                        case "3":
                            Console.Clear();
                            Console.WriteLine(languageJson.StartSave.Name);
                            string saveName = Console.ReadLine();
                            bool successUse = FileSaveViewModel.Save(saveName);
                            if (successUse)
                            {
                                Console.WriteLine(languageJson.StartSave.StartProcess);
                            }
                            else
                            {
                                Console.WriteLine(languageJson.StartSave.MissingFile);
                            }
                            Console.WriteLine(languageJson.StartSave.Continue);
                            Console.ReadLine();
                            Console.Clear();
                            break;
                            
                        case "4":
                            Console.Clear();
                            Console.WriteLine(languageJson.AllProcess.StartProcess);
                            int errors = FileSaveViewModel.AllSave();
                            if (errors == 0)
                            {
                                Console.WriteLine(languageJson.AllProcess.EndProcess);
                            }
                            else
                            {
                                Console.WriteLine("=> " + errors + languageJson.AllProcess.Error);
                            }
                            Console.WriteLine(languageJson.AllProcess.Continue);
                            Console.ReadLine();
                            Console.Clear();
                            break;
                            
                        case "5":
                            Console.Clear();
                            foreach (var saveWork in FileSaveViewModel.GetAvailableWorks())
                            {
                                Console.WriteLine("=> " + saveWork);
                            }
                            Console.WriteLine(languageJson.BackupDetails.Continue);
                            Console.ReadLine();
                            Console.Clear();
                            break;
                            
                        case "6":
                            Environment.Exit(0);
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

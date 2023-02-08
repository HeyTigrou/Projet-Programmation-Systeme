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
        private enum languages { en, fr } // this enum will be used to check if the user input is valid when he select the language

        // 1. This part is used to make the class a singleton
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
        // .1

        public void Display() // this method will be called in the main method of the program
        {
            string Language = "";
            do
            {
                Console.Clear();
                Console.WriteLine("*** Welcome to EasySave ***\n ");
                Console.WriteLine("Choose a language: \n> English => en \n> French => fr");
                Console.Write("\nEnter an option: ");
                Language = Console.ReadLine(); // the user will enter the language he wants to use
            }
            while (!(Enum.IsDefined(typeof(languages), Language))); // this loop will be executed while the user input is not valid
            while (true) // this loop will be executed until the user wants to quit the program
            {
                dynamic languageJson = JsonConvert.DeserializeObject(File.ReadAllText($@"..\..\..\..\easy_save.Lib\ConfigurationFiles\Interface_text\{Language}.json")); // this line will get the json file corresponding to the language selected by the user

                bool inputIsOk;
                do
                {
                    inputIsOk = true;

                    Console.Clear();
                    Console.WriteLine("*** EasySave ***\n ");
                    Console.WriteLine(languageJson.Main.Menu);
                    Console.Write(languageJson.Main.Option);
                    string inputChoice = Console.ReadLine(); // the user will enter the option he wants to use


                    switch (inputChoice)
                    {
                        case "1": // the user wants to create a save work
                            Console.Clear();
                            Console.WriteLine(languageJson.Save.Name);
                            string name = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.PathSave);
                            string inputPath = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.PathDestination);
                            string outputPath = Console.ReadLine();
                            Console.WriteLine(languageJson.Save.SaveType);
                            int saveType = int.Parse(Console.ReadLine());
                            bool success = FileSaveViewModel.CreateSaveWork(inputPath, outputPath, name, saveType); // this method will create the save work via the view model
                            if (success)
                            {
                                Console.WriteLine(languageJson.Save.Process); // the save work has been created
                            }
                            else
                            {
                                Console.WriteLine(languageJson.Save.ErrorLimit); // the user has reached the limit of save works he can create
                            }
                            Console.WriteLine(languageJson.Save.Continue);  // the user will have to press enter to continue
                            Console.ReadLine();
                            break;

                        case "2": // the user wants to delete a save work
                            Console.Clear();
                            Console.WriteLine(languageJson.Remove.Name);
                            string nameToDelete = Console.ReadLine();
                            bool successDelete = FileSaveViewModel.DeleteSaveWork(nameToDelete); // this method will delete the save work via the view model
                            if (successDelete)
                            {
                                Console.WriteLine(languageJson.Remove.SucessRemove); // the save work has been deleted
                            }
                            else
                            {
                                Console.WriteLine(languageJson.Remove.MissingFile); // the save work doesn't exist
                            }
                            Console.WriteLine(languageJson.Remove.Continue); // the user will have to press enter to continue
                            Console.ReadLine();
                            break;

                        case "3": // the user wants to use a save work
                            Console.Clear();
                            Console.WriteLine(languageJson.StartSave.Name);
                            string saveName = Console.ReadLine();
                            bool successUse = FileSaveViewModel.Save(saveName); // this method will use the save work via the view model
                            if (successUse)
                            {
                                Console.WriteLine(languageJson.StartSave.StartProcess); // the save work has been used
                            }
                            else
                            {
                                Console.WriteLine(languageJson.StartSave.MissingFile); // the save work doesn't exist
                            }
                            Console.WriteLine(languageJson.StartSave.Continue); // the user will have to press enter to continue
                            Console.ReadLine();
                            Console.Clear();
                            break;

                        case "4": // the user wants to use all save works
                            Console.Clear();
                            Console.WriteLine(languageJson.AllProcess.StartProcess);
                            int errors = FileSaveViewModel.AllSave(); // this method will use all save works via the view model
                            if (errors == 0)
                            {
                                Console.WriteLine(languageJson.AllProcess.EndProcess); // all save works have been used
                            }
                            else
                            {
                                Console.WriteLine("=> " + errors + languageJson.AllProcess.Error); // some save works have not been used, the number of errors is displayed
                            }
                            Console.WriteLine(languageJson.AllProcess.Continue);
                            Console.ReadLine();
                            Console.Clear();
                            break;

                        case "5": // the user wants to see the details of a save work
                            Console.Clear();
                            foreach (var saveWork in FileSaveViewModel.GetAvailableWorks()) // this method will get the details of each save work via the view model
                            {
                                Console.WriteLine("=> " + saveWork);
                            }
                            Console.WriteLine(languageJson.BackupDetails.Continue); // the user will have to press enter to continue
                            Console.ReadLine();
                            Console.Clear();
                            break;

                        case "6": // the user wants to quit the program
                            Environment.Exit(0); 
                            break;

                        default: // the user input is not valid
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

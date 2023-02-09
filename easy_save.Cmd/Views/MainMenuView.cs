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
        // this enum will be used to check if the user input is valid when he select the language
        private enum Languages { en, fr } 

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

        // This is called when we need to check if the user input isn't null or empty
        public static string CheckInput(string question, string error)
        {
            while (true)
            {
                Console.WriteLine(question);
                string? input = Console.ReadLine();
                if (input != "" && input != null)
                {
                    return input;
                }
                Console.Clear();
                Console.WriteLine(error);
            }
        }

        // this method will be called in the main method of the program
        public void Display()
        {
            string? language;
            do
            {
                Console.Clear();
                Console.WriteLine("*** Welcome to EasySave ***\n ");
                Console.WriteLine("Choose a language: \n> English => en \n> French => fr");
                Console.Write("\nEnter an option: ");
                // the user will enter the language he wants to use
                language = Console.ReadLine();
            }
            // this loop will be executed while the user input is not valid
            while (!Enum.IsDefined(typeof(Languages), language));
            // this loop will be executed until the user wants to quit the program
            while (true) 
            {
                // this line will get the json file corresponding to the language selected by the user
                dynamic languageJson = JsonConvert.DeserializeObject(File.ReadAllText($@"..\..\..\ConfigurationFiles\Interface_text\{language}.json")); 

                bool inputIsOk;
                do
                {
                    inputIsOk = true;

                    Console.Clear();
                    Console.WriteLine("*** EasySave ***\n ");
                    Console.WriteLine(languageJson.Main.Menu);
                    Console.Write(languageJson.Main.Option);
                    // the user will enter the option he wants to use
                    string inputChoice = Console.ReadLine(); 
                    switch (inputChoice)
                    {
                        // the user wants to create a save work
                        case "1":
                            Console.Clear();
                            string name = CheckInput(languageJson.Save.Name.ToString(), languageJson.Save.ErrorInput.ToString());
                            string inputPath = CheckInput(languageJson.Save.PathSave.ToString(), languageJson.Save.ErrorInput.ToString());
                            string outputPath = CheckInput(languageJson.Save.PathDestination.ToString(), languageJson.Save.ErrorInput.ToString());
                            int saveType;
                            while (true)
                            {
                                Console.WriteLine(languageJson.Save.SaveType);
                                saveType = int.Parse(Console.ReadLine());
                                // the user can only choose between 0 and 1 (full or incremental)
                                if (saveType == 0 || saveType == 1) 
                                {
                                    break;
                                }
                                Console.Clear();
                                Console.WriteLine(languageJson.Save.ErrorSaveType);
                            }
                            
                            // this method will create the save work via the view model
                            bool success = FileSaveViewModel.CreateSaveWork(inputPath, outputPath, name, saveType); 
                            if (success)
                            {
                                // the save work has been created
                                Console.WriteLine(languageJson.Save.Process); 
                            }
                            else
                            {
                                // the user has reached the limit of save works he can create
                                Console.WriteLine(languageJson.Save.ErrorLimit); 
                            }
                            // the user will have to press enter to continue
                            Console.WriteLine(languageJson.Save.Continue);  
                            Console.ReadLine();
                            break;

                        // the user wants to delete a save work
                        case "2": 
                            Console.Clear();
                            string nameToDelete = CheckInput(languageJson.Remove.Name.ToString(), languageJson.Save.ErrorInput.ToString());
                            // this method will delete the save work via the view model
                            bool successDelete = FileSaveViewModel.DeleteSaveWork(nameToDelete); 
                            if (successDelete)
                            {
                                // the save work has been deleted
                                Console.WriteLine(languageJson.Remove.SucessRemove); 
                            }
                            else
                            {
                                // the save work doesn't exist
                                Console.WriteLine(languageJson.Remove.MissingFile);
                            }
                            // the user will have to press enter to continue
                            Console.WriteLine(languageJson.Remove.Continue); 
                            Console.ReadLine();
                            break;

                        // the user wants to use a save work
                        case "3": 
                            Console.Clear();
                            string saveName = CheckInput(languageJson.StartSave.Name.ToString(), languageJson.Save.ErrorInput.ToString());
                            // this method will use the save work via the view model
                            bool successUse = FileSaveViewModel.Save(saveName); 
                            if (successUse)
                            {
                                // the save work has been used
                                Console.WriteLine(languageJson.StartSave.StartProcess); 
                            }
                            else
                            {
                                // the save work doesn't exist
                                Console.WriteLine(languageJson.StartSave.Error); 
                            }
                            // the user will have to press enter to continue
                            Console.WriteLine(languageJson.StartSave.Continue); 
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        
                        // the user wants to use all save works
                        case "4": 
                            Console.Clear();
                            Console.WriteLine(languageJson.AllProcess.StartProcess);
                            // this method will use all save works via the view model
                            int errors = FileSaveViewModel.AllSave(); 
                            if (errors == 0)
                            {
                                // all save works have been used
                                Console.WriteLine(languageJson.AllProcess.EndProcess); 
                            }
                            else
                            {
                                // some save works have not been used, the number of errors is displayed
                                Console.WriteLine("=> " + errors + languageJson.AllProcess.Error); 
                            }
                            Console.WriteLine(languageJson.AllProcess.Continue);
                            Console.ReadLine();
                            Console.Clear();
                            break;

                        // the user wants to see the details of a save work
                        case "5": 
                            Console.Clear();
                            // this method will get the details of each save work via the view model
                            foreach (var saveWork in FileSaveViewModel.GetAvailableWorks()) 
                            {
                                Console.WriteLine("=> " + saveWork);
                            }
                            // the user will have to press enter to continue
                            Console.WriteLine(languageJson.BackupDetails.Continue); 
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        
                        // the user wants to quit the program
                        case "6": 
                            Environment.Exit(0); 
                            break;

                        // the user input is not valid
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

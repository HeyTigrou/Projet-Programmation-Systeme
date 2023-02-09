using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using easy_save.Cmd.ConfigurationFiles.Interface_text;
using easy_save.Lib.ViewModels;
using easy_save.Lib.Service;
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

                ChoosenLanguage choosenLanguage = new();
                choosenLanguage.GetLanguage(language);
                bool inputIsOk;
                do
                {
                    
                    inputIsOk = true;

                    Console.Clear();
                    Console.WriteLine("*** EasySave ***\n ");
                    Console.WriteLine(choosenLanguage.Main_Menu);
                    Console.Write(choosenLanguage.Main_Option);
                    // the user will enter the option he wants to use
                    string inputChoice = Console.ReadLine(); 
                    switch (inputChoice)
                    {
                        // the user wants to create a save work
                        case "1":
                            Console.Clear();
                            string name = CheckInput(choosenLanguage.Save_Name, choosenLanguage.Save_ErrorInput);
                            string inputPath = CheckInput(choosenLanguage.Save_PathSave, choosenLanguage.Save_ErrorInput);
                            string outputPath = CheckInput(choosenLanguage.Save_PathDestination, choosenLanguage.Save_ErrorInput);
                            int saveType;
                            while (true)
                            {
                                Console.WriteLine(choosenLanguage.Save_SaveType);
                                saveType = int.Parse(Console.ReadLine());
                                // the user can only choose between 0 and 1 (full or incremental)
                                if (saveType == 0 || saveType == 1) 
                                {
                                    break;
                                }
                                Console.Clear();
                                Console.WriteLine(choosenLanguage.Save_ErrorSaveType);
                            }
                            
                            // this method will create the save work via the view model
                            bool success = FileSaveViewModel.CreateSaveWork(inputPath, outputPath, name, saveType); 
                            if (success)
                            {
                                // the save work has been created
                                Console.WriteLine(choosenLanguage.Save_Process); 
                            }
                            else
                            {
                                // the user has reached the limit of save works he can create
                                Console.WriteLine(choosenLanguage.Save_ErroLimit); 
                            }
                            // the user will have to press enter to continue
                            Console.WriteLine(choosenLanguage.Save_Continue);  
                            Console.ReadLine();
                            break;

                        // the user wants to delete a save work
                        case "2": 
                            Console.Clear();
                            string nameToDelete = CheckInput(choosenLanguage.Remove_Name, choosenLanguage.Save_ErrorInput);
                            // this method will delete the save work via the view model
                            bool successDelete = FileSaveViewModel.DeleteSaveWork(nameToDelete); 
                            if (successDelete)
                            {
                                // the save work has been deleted
                                Console.WriteLine(choosenLanguage.Remove_SucessRemove); 
                            }
                            else
                            {
                                // the save work doesn't exist
                                Console.WriteLine(choosenLanguage.Remove_MissingFile);
                            }
                            // the user will have to press enter to continue
                            Console.WriteLine(choosenLanguage.Remove_Continue); 
                            Console.ReadLine();
                            break;

                        // the user wants to use a save work
                        case "3": 
                            Console.Clear();
                            string saveName = CheckInput(choosenLanguage.StartSave_Name, choosenLanguage.Save_ErrorInput);
                            // this method will use the save work via the view model
                            bool successUse = FileSaveViewModel.Save(saveName); 
                            if (successUse)
                            {
                                // the save work has been used
                                Console.WriteLine(choosenLanguage.StartSave_StartProcess); 
                            }
                            else
                            {
                                // the save work doesn't exist
                                Console.WriteLine(choosenLanguage.StartSave_Error); 
                            }
                            // the user will have to press enter to continue
                            Console.WriteLine(choosenLanguage.StartSave_Continue); 
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        
                        // the user wants to use all save works
                        case "4": 
                            Console.Clear();
                            Console.WriteLine(choosenLanguage.AllProcess_StartProcess);
                            // this method will use all save works via the view model
                            int errors = FileSaveViewModel.AllSave(); 
                            if (errors == 0)
                            {
                                // all save works have been used
                                Console.WriteLine(choosenLanguage.AllProcess_EndProcess); 
                            }
                            else
                            {
                                // some save works have not been used, the number of errors is displayed
                                Console.WriteLine("=> " + errors + choosenLanguage.AllProcess_Error); 
                            }
                            Console.WriteLine(choosenLanguage.AllProcess_Continue);
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
                            Console.WriteLine(choosenLanguage.BackupDetails_Continue); 
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

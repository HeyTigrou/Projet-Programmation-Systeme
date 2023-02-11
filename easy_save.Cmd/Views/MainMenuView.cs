using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using easy_save.Lib.ViewModels;
using easy_save.Lib.Service;
using Newtonsoft.Json;
using System.Resources;
using easy_save.Cmd.Resources;
using System.Security.AccessControl;

namespace easy_save.Cmd.Views
{
    public class MainMenuView
    {
        private enum Languages { en, fr}

        private enum SaveChoices
        {
            Complete = 0,
            Incremental = 1
        }

        // This method will create our display
        public void Display()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            SetLanguages();

            while (true)
            {
                bool inputIsOk;
                do
                {
                    inputIsOk = true;

                    Console.Clear();
                    Console.WriteLine("*** EasySave ***\n ");
                    Console.WriteLine(Strings.Main_Menu);
                    Console.Write(Strings.Main_Option);
                    string inputChoice = Console.ReadLine();
                    switch (inputChoice)
                    {
                        case "1":
                            CreateSaveWork();
                            break;

                        case "2":
                            DeleteSaveWork();
                            break;

                        case "3":
                            LaunchSaveWork();
                            break;

                        case "4":
                            LaunchALLSaveWorks();
                            break;

                        case "5":
                            Console.Clear();
                            Console.WriteLine("*** Welcome to EasySave ***\n ");
                            DisplayAllWorks();
                            Console.WriteLine(Strings.BackupDetails_Continue);
                            Console.ReadLine();
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

        // This method is used to check if the input is correct, it will ask which parameter you want to enter and will return the input
        public string CheckInput(string question, string error, bool display)
        {
            if (display) // If display is true, it will display all the save works
            {
                DisplayAllWorks();
            }
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

        // This method is used to ask the user which language he wants to use
        private void SetLanguages()
        { 
            string? language;
            do
            {
                Console.Clear();
                Console.WriteLine("*** Welcome to EasySave ***\n ");
                Console.WriteLine("Choose a Strings: \n> English => en \n> French => fr");
                Console.Write("\nEnter an option: ");

                 language = Console.ReadLine();  
            }
            while (!Enum.IsDefined(typeof(Languages), language));

            Strings.Culture = new CultureInfo(language);
        }

        // Create a new save work Display
        private void CreateSaveWork()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("*** EasySave ***\n ");
                string name = CheckInput(Strings.Save_Name, Strings.Save_ErrorInput, false);
                string inputPath = CheckInput(Strings.Save_PathSave, Strings.Save_ErrorInput, false);
                string outputPath = CheckInput(Strings.Save_PathDestination, Strings.Save_ErrorInput, false);
                int saveType;
                while (true)
                {
                    Console.WriteLine(Strings.Save_SaveType);
                    
                    int.TryParse(Console.ReadLine(), out saveType);

                    if (saveType is (int)SaveChoices.Complete or (int)SaveChoices.Incremental)
                    {
                        break;
                    }

                    Console.Clear();
                    Console.WriteLine(Strings.Save_ErrorSaveType);
                }
                
                Console.WriteLine($"\n {Strings.ParameterPresentation} : \n =>\t{Strings.Save_SaveName} {name}\n =>\t{Strings.Save_InputPath} {inputPath}\n =>\t{Strings.Save_OutputPath} {outputPath}\n =>\t{Strings.Save_SaveType2} {saveType}\n");

                Console.WriteLine(Strings.Save_ConfirmChoice);
                string input = Console.ReadLine();
                if (input == Strings.ConfirmWord)
                {
                    bool success = FileSaveViewModel.CreateSaveWork(inputPath, outputPath, name, saveType);
                    if (success)
                    {
                        Console.WriteLine(Strings.Save_Process);
                    }
                    else
                    {
                        Console.WriteLine(Strings.Save_ErroLimit);
                    }
                    Console.WriteLine(Strings.Save_Continue);
                    Console.ReadLine();
                    break;
                }
                else if (input == Strings.CancelWord)
                {
                    Console.WriteLine($"\n=>{Strings.Save_Cancel}");
                    Console.ReadLine();
                    break;
                }
            } 
        }

        // Delete a save work Display
        private void DeleteSaveWork()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("*** EasySave ***\n ");

                string nameToDelete = CheckInput(Strings.Remove_Name, Strings.Save_ErrorInput, true);

                Console.WriteLine($"{Strings.Remove_ConfirmChoice} {nameToDelete}{Strings.Remove_ConfirmChoice2}");
                string input = Console.ReadLine();
                if (input == Strings.ConfirmWord)
                {
                    bool successDelete = FileSaveViewModel.DeleteSaveWork(nameToDelete);
                    if (successDelete)
                    {
                        Console.WriteLine(Strings.Remove_SucessRemove);
                    }
                    else
                    {
                        Console.WriteLine(Strings.Remove_MissingFile);
                    }
                    Console.WriteLine(Strings.Remove_Continue);
                    Console.ReadLine();
                    break;
                }
                else if (input == Strings.CancelWord)
                {
                    Console.WriteLine($"\n =>{Strings.Remove_Cancel}");
                    Console.ReadLine();
                    break;
                }
            } 
        }

        // Launch a save work Display
        private void LaunchSaveWork()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("*** EasySave ***\n ");

                string saveName = CheckInput(Strings.StartSave_Name, Strings.Save_ErrorInput, true);

                Console.WriteLine($"{Strings.SaveStart_ConfirmChoice} {saveName}{Strings.SaveStart_ConfirmChoice2}");
                string input = Console.ReadLine();
                if (input == Strings.ConfirmWord)
                {
                    Console.WriteLine($"\n{Strings.StartSave_StartProcess}");
                    bool successUse = FileSaveViewModel.Save(saveName);
                    if (successUse)
                    {
                        Console.WriteLine(Strings.StartSave_Sucess);
                    }
                    else
                    {
                        Console.WriteLine(Strings.StartSave_Error);
                    }
                    Console.WriteLine(Strings.StartSave_Continue);
                    Console.ReadLine();
                    Console.Clear();
                    break;
                }
                else if (input == Strings.CancelWord)
                {
                    Console.WriteLine($"\n => {Strings.StartSave_Cancel}");
                    Console.ReadLine();
                    break;
                }
            }
        }

        // Launch all save works Display
        private void LaunchALLSaveWorks()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("*** EasySave ***\n ");
                Console.WriteLine(Strings.AllProcess_StartProcess);
                string input = Console.ReadLine();
                if (input == Strings.ConfirmWord)
                {
                    int errors = FileSaveViewModel.AllSave();
                    if (errors == 0)
                    {
                        Console.WriteLine(Strings.AllProcess_EndProcess);
                    }
                    else
                    {
                        Console.WriteLine($"=> {errors} {Strings.AllProcess_Error}");
                    }
                    Console.WriteLine(Strings.AllProcess_Continue);
                    Console.ReadLine();
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.WriteLine($"\n => {Strings.AllProcess_Cancel}");
                    Console.ReadLine();
                    break;
                }
            }
        }

        // Display all save works
        private void DisplayAllWorks()
        {
            string[] saveWorks = FileSaveViewModel.GetAvailableWorks();
            if (saveWorks.Length != 0)
            {
                Console.WriteLine(Strings.BackupDetails_Presentation);
                foreach (var saveWork in saveWorks)
                {
                    Console.WriteLine($"=> {saveWork}");
                }
            }
            else
            {
                Console.WriteLine(Strings.BackupDetails_Empty);
            }
            Console.WriteLine();
        }  
    }
}

using easy_save.Lib.Models;
using easy_save.Lib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace easy_save.Lib.ViewModels
{
    public class FileSaveViewModel
    {
        public static bool Save(string name) // This method is used to launch a specific save work
        {
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) // We get all the save works
            {
                if (saveWork.Name == name) // We check if the name of the save work is the same as the name that we selected
                {
                    FileSaveService.SaveProcess(saveWork); // We launch the save process
                    return true; // We return true if the save process is launched
                }

            }
            return false; // We return false if the save process is not launched
        }

        public static int AllSave() // This method is used to launch all the save works
        {
            int errorCount = 0; // We create a variable to count the number of errors
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) // We get all the save works
            {
                try // We try to launch the save process
                {
                    FileSaveService.SaveProcess(saveWork);
                }
                catch (Exception e) // If there is an error, we increment the error count
                {
                    errorCount++;
                }
            }
            return errorCount; // We return the number of errors
        }

        public static bool CreateSaveWork(string inputPath, string outputPath, string name, int saveType) // This method is used to create a save work
        {
            SaveWorkModel saveWork = new SaveWorkModel(name, inputPath, outputPath, saveType); // We create a new save work
            return SaveWorkManagerService.AddSaveWork(saveWork); // We add the save work to the save work list and we return if it's a success or not
        }

        public static bool DeleteSaveWork(string name) // This method is used to delete a save work
        {
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) // We get all the save works
            {
                if (saveWork.Name == name) // We check if the name of the save work is the same as the name that we selected
                {
                    return SaveWorkManagerService.DeleteSaveWork(name); // We delete the save work and we return if it's a success or not
                }
                
            }
            return false; // We return false if the save work can't be found
        }

        public static string[] GetAvailableWorks() // This method is used to get all the save works
        {
            string[] works = new string[SaveWorkManagerService.GetSaveProjectnumber()]; // We create a string array with the number of save works
            int i = 0;
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) // We get all the save works
            {
                works[i] = $"Name: {saveWork.Name} | Source: {saveWork.InputPath} | Target: {saveWork.OutputPath} | SaveType: {saveWork.SaveType}"; // We add the save work to the string array
                i++;
            }
            return works; // We return the string array
        }
    }
}

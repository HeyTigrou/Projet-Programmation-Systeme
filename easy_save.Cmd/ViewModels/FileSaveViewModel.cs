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
        // This method is used to launch a specific save work
        public static bool Save(string name) 
        {
            // We get all the save works
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) 
            {
                // We check if the name of the save work is the same as the name that we selected
                if (saveWork.Name == name) 
                {
                    try
                    {
                        // We launch the save process
                        FileSaveService fileSaveService = new FileSaveService();
                        fileSaveService.SaveProcess(saveWork);
                        // We return true if the save process is launched
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                    
                }
            }
            // We return false if the save process is not launched
            return false; 
        }

        // This method is used to launch all the save works
        public static int AllSave() 
        {
            // We create a variable to count the number of errors
            int errorCount = 0;
            // We get all the save works
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) 
            {
                // We try to launch the save process
                try
                {
                    FileSaveService fileSaveService = new FileSaveService();
                    fileSaveService.SaveProcess(saveWork);
                }
                // If there is an error, we increment the error count
                catch (Exception e) 
                {
                    errorCount++;
                }
            }
            // We return the number of errors
            return errorCount; 
        }

        // This method is used to create a save work
        public static bool CreateSaveWork(string inputPath, string outputPath, string name, int saveType) 
        {
            // We create a new save work
            SaveWorkModel saveWork = new SaveWorkModel { Name = name, InputPath = inputPath, OutputPath = outputPath, SaveType = saveType };
            // We add the save work to the save work list and we return if it's a success or not
            return SaveWorkManagerService.AddSaveWork(saveWork); 
        }

        // This method is used to delete a save work
        public static bool DeleteSaveWork(string name) 
        {
            // We get all the save works
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) 
            {
                // We check if the name of the save work is the same as the name that we selected
                if (saveWork.Name == name) 
                {
                    // We delete the save work and we return if it's a success or not
                    return SaveWorkManagerService.DeleteSaveWork(name); 
                }
                
            }
            // We return false if the save work can't be found
            return false; 
        }

        // This method is used to get all the save works
        public static string[] GetAvailableWorks() 
        {
            // We create a string array with the number of save works
            string[] works = new string[SaveWorkManagerService.GetSaveProjectnumber()]; 
            int i = 0;
            // We get all the save works
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks()) 
            {
                // We add the save work to the string array
                works[i] = $"Name: {saveWork.Name} | Source: {saveWork.InputPath} | Target: {saveWork.OutputPath} | SaveType: {saveWork.SaveType}"; 
                i++;
            }
            // We return the string array
            return works; 
        }
    }
}

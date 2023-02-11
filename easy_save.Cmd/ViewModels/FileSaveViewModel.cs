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
            SaveWorkManagerService saveWorkManagerService = new SaveWorkManagerService();
            foreach (var saveWork in saveWorkManagerService.GetSaveWorks()) 
            {
                if (saveWork.Name == name) 
                {
                    try
                    {
                        FileSaveService fileSaveService = new FileSaveService();
                        fileSaveService.SaveProcess(saveWork);
                        
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false; 
        }

        // This method is used to launch all the save works
        public static int AllSave() 
        {
            SaveWorkManagerService saveWorkManagerService = new SaveWorkManagerService();
            int errorCount = 0;
            
            foreach (var saveWork in saveWorkManagerService.GetSaveWorks()) 
            {
                try
                {
                    FileSaveService fileSaveService = new FileSaveService();
                    fileSaveService.SaveProcess(saveWork);
                }
                
                catch
                {
                    errorCount++;
                }
            }
            
            return errorCount; 
        }

        // This method is used to create a save work
        public static bool CreateSaveWork(string inputPath, string outputPath, string name, int saveType) 
        {
            SaveWorkManagerService saveWorkManagerService = new SaveWorkManagerService();
            SaveWorkModel saveWork = new SaveWorkModel { Name = name, InputPath = inputPath, OutputPath = outputPath, SaveType = saveType };

            return saveWorkManagerService.AddSaveWork(saveWork); // AddSaveWork returns a boolean, true = success, false = error
        }

        // This method is used to delete a save work
        public static bool DeleteSaveWork(string name) 
        {
            SaveWorkManagerService saveWorkManagerService = new SaveWorkManagerService();
            foreach (var saveWork in saveWorkManagerService.GetSaveWorks()) 
            {
                if (saveWork.Name == name) 
                {
                    return saveWorkManagerService.DeleteSaveWork(name); // DeleteSaveWork returns a boolean, true = success, false = error
                }
            }
            return false; 
        }

        // This method is used to get all the save works
        public static string[] GetAvailableWorks()
        {
            SaveWorkManagerService saveWorkManagerService = new SaveWorkManagerService();
            string[] works = new string[saveWorkManagerService.GetSaveProjectnumber()]; 

            SaveWorkModel[] saveWorks = saveWorkManagerService.GetSaveWorks();
            for (int i = 0; i < works.Length; i++)
            {
                works[i] = $"Name: {saveWorks[i].Name} | Source: {saveWorks[i].InputPath} | Target: {saveWorks[i].OutputPath} | SaveType: {saveWorks[i].SaveType}";
            }
            
            return works; 
        }
    }
}

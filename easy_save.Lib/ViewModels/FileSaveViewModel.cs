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
        public static bool Save(string name)
        {
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks())
            {
                if (saveWork.Name == name)
                {
                    FileSaveService.SaveProcess(saveWork);
                    return true;
                }

            }
            return false;
        }

        public static int AllSave()
        {
            int errorCount = 0;
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks())
            {
                try
                {
                    FileSaveService.SaveProcess(saveWork);
                }
                catch (Exception e)
                {
                    errorCount++;
                }
            }
            return errorCount;
        }

        public static bool CreateSaveWork(string inputPath, string outputPath, string name, int saveType)
        {
            SaveWorkModel saveWork = new SaveWorkModel(name, inputPath, outputPath, saveType);

            return SaveWorkManagerService.AddSaveWork(saveWork);
        }

        public static bool DeleteSaveWork(string name)
        {
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks())
            {
                if (saveWork.Name == name)
                {
                    return SaveWorkManagerService.DeleteSaveWork(name);
                }
                
            }
            return false;
        }

        public static string[] GetAvailableWorks()
        {
            string[] works = new string[SaveWorkManagerService.GetSaveProjectnumber()];
            int i = 0;
            foreach (var saveWork in SaveWorkManagerService.GetSaveWorks())
            {
                works[i] = $"Name: {saveWork.Name} | Source: {saveWork.InputPath} | Target: {saveWork.OutputPath} | SaveType: {saveWork.SaveType}";
                i++;
            }
            return works;
        }
    }
}

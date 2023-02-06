using easy_save.Lib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace easy_save.Lib.ViewModels
{
    public class FileSaveViewModel
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string Name { get; set; }
        public int SaveType { get; set; }

        public void Save()
        {
            FileSaveService service = new FileSaveService(this.InputPath, this.OutputPath, this.Name);
            service.SaveProcess(this.SaveType);
        }

        public static string[] GetAvailableWorks()
        {
            string[] works = new string[Directory.GetFiles(@"..\..\..\..\easy_save.Lib\ConfigurationFiles\SaveProjects\", "*.json", SearchOption.AllDirectories).Length];
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

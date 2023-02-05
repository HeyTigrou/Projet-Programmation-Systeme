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
            Console.WriteLine("Save "+ this.Name+" : " + this.InputPath + " to " + this.OutputPath + " with type " + this.SaveType);
        }
    }
}

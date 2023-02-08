using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    public class SaveWorkModel // This Model is used to manage the save works
    {
        public string Name { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public int SaveType { get; set; }

        public SaveWorkModel(string name, string inputPath, string outputPath, int saveType) // Constructor
        {
            Name = name;
            InputPath = inputPath;
            OutputPath = outputPath;
            SaveType = saveType;
        }
    }
}

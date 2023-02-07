using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    public class SaveWorkModel
    {
        public string Name { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public int SaveType { get; set; }

        public string State { get; set; }
        public double TotalFilesToCopy { get; set; }
        public double TotalFilesSize { get; set; }
        public double NbFilesLeftToDo { get; set; }
        public int Progression { get; set; }

        public SaveWorkModel(string name, string inputPath, string outputPath, int saveType)
        {
            Name = name;
            InputPath = inputPath;
            OutputPath = outputPath;
            SaveType = saveType;
        }

        public SaveWorkModel(string name, string inputPath, string outputPath, int saveType, string state, double totalFilesToCopy, double totalFilesSize, double nbFilesLeftToDo, int progression)
        {
            Name = name;
            InputPath = inputPath;
            OutputPath = outputPath;
            SaveType = saveType;
            State = state;
            TotalFilesToCopy = totalFilesToCopy;
            TotalFilesSize = totalFilesSize;
            NbFilesLeftToDo = nbFilesLeftToDo;
            Progression = progression;
        }

        public SaveWorkModel() {  }
        
    }
}

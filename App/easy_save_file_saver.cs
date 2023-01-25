using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave
{
    public class EasySaveFileSaver
    {
        private string sourcePath;
        private string targetPath;
        private string name;
        private string type;

        public EasySaveFileSaver(string sourcePath = "", string targetPath = "", string name = "", string type = "")
        {
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
            this.name = name;
            this.type = type;
        }

        private void saveFile(string sourcePath, string targetPath)
        {
           try
            {
                File.Copy(sourcePath, targetPath, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public void saveProcess(string type)
        {
            if (type == "Complete")
            {
                saveAllFiles();
            }
            else if (type == "Incremental")
            {
                saveChangedFiles();
            }
        }

        private void saveAllFiles()
        {
            foreach (var file in Directory.GetFiles(this.sourcePath))
                File.Copy(file, Path.Combine(this.targetPath, Path.GetFileName(file)));
        }

        private void saveChangedFiles()
        {
            DateTime lackSaveTime = DateTime.MinValue;

            foreach(var file in Directory.GetFiles(this.sourcePath))
            {   
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.LastAccessTime > lackSaveTime)
                {
                    File.Copy(file, Path.Combine(this.targetPath, Path.GetFileName(file)), true);
                }
            }
            lackSaveTime = DateTime.Now;
        }
    }
}

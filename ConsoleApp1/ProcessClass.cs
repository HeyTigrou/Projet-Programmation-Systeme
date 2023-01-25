using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc
{
    public class ProcessClass
    {
        private string sourcePath;
        private string targetPath;
        private string name;
        private string type;

        public ProcessClass(string sourcePath = "", string targetPath = "", string name = "", string type = "")
        {
            this.sourcePath = sourcePath;
            this.targetPath = targetPath;
            this.name = name;
            this.type = type;
        }

        private void copy(string sourcePath, string targetPath)
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
        
        public void process(string type)
        {
            if (type == "Complete")
            {
                CompleteProcess();
            }
            else if (type == "Incremental")
            {
                IncrementalProcess();
            }
        }

        private void CompleteProcess()
        {
            foreach (var file in Directory.GetFiles(this.sourcePath))
                File.Copy(file, Path.Combine(this.targetPath, Path.GetFileName(file)));
        }

        private void IncrementalProcess()
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

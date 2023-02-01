using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Enumeration;
using System.Security.Cryptography;

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
        
        public void saveProcess(string type)
        {
            if (type == "Complete")
            {
                saveAllFiles(this.sourcePath, this.targetPath);
            }
            else if (type == "Incremental")
            {
                saveChangedFiles(this.sourcePath, this.targetPath);
            }
        }
        
        private void saveAllFiles(string sourcePath, string destinationPath)
        {  
            foreach ( string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }
        
        

        private void saveChangedFiles(string sourcePath, string destinationPath)
        {
            
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(dirPath);
                DirectoryInfo destinationDirectoryInfo = new DirectoryInfo(dirPath.Replace(sourcePath, destinationPath));

                if ( sourceDirectoryInfo.LastWriteTime > destinationDirectoryInfo.LastWriteTime)
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                    Console.WriteLine("Directory : " + dirPath);
                }  
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {

                Console.WriteLine(newPath.Replace(sourcePath, destinationPath));

                FileInfo sourceFileInfo = new FileInfo(newPath);
                FileInfo destinationFileInfo = new FileInfo(newPath.Replace(sourcePath, destinationPath));

                if (sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }
            }
        }    
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Enumeration;
using System.Security.Cryptography;
using easy_save.Lib.Models;

namespace easy_save.Lib.Service
{
    public class FileSaveService
    {
        public static void SaveProcess(SaveWorkModel save)
        {
            if (save.SaveType == 0)
            {
                SaveAllFiles(save.InputPath, save.OutputPath);
            }
            else if (save.SaveType == 1)
            {
                SaveChangedFiles(save.InputPath, save.OutputPath);
            }
        }

        private static void SaveAllFiles(string sourcePath, string destinationPath)
        {
            LoggerService logger = new LoggerService("logs"); // !!!!!!!!
            
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }

        private static void SaveChangedFiles(string sourcePath, string destinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(dirPath);
                DirectoryInfo destinationDirectoryInfo = new DirectoryInfo(dirPath.Replace(sourcePath, destinationPath));
                if (!destinationDirectoryInfo.Exists)
                {
                    destinationDirectoryInfo.Create();
                }
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                FileInfo sourceFileInfo = new FileInfo(newPath);
                FileInfo destinationFileInfo = new FileInfo(newPath.Replace(sourcePath, destinationPath));
                if (!destinationFileInfo.Exists || sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }
            }
        }
    }
}

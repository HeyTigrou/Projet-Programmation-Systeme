using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Enumeration;
using System.Security.Cryptography;

namespace easy_save.Lib.Service
{
    public class FileSaveService
    {
        private string SourcePath;
        private string TargetPath;
        private string Name;

        public FileSaveService(string sourcePath = "", string targetPath = "", string name = "")
        {
            this.SourcePath = sourcePath;
            this.TargetPath = targetPath;
            this.Name = name;
        }

        public void SaveProcess(int type)
        {
            if (type == 0)
            {
                SaveAllFiles(this.SourcePath, this.TargetPath);
            }
            else if (type == 1)
            {
                SaveChangedFiles(this.SourcePath, this.TargetPath);
            }
        }

        private void SaveAllFiles(string sourcePath, string destinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }

        private void SaveChangedFiles(string sourcePath, string destinationPath)
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

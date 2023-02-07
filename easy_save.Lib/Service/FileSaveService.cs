using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Enumeration;
using System.Security.Cryptography;
using easy_save.Lib.Models;
using System.Drawing;
using System.Diagnostics.Metrics;

namespace easy_save.Lib.Service
{
    public class FileSaveService
    {
        public static void SaveProcess(SaveWorkModel save)
        {
            if (save.SaveType == 0)
            {
                SaveAllFiles(save.InputPath, save.OutputPath, save.Name);
            }
            else if (save.SaveType == 1)
            {
                SaveChangedFiles(save.InputPath, save.OutputPath, save.Name);
            }
        }

        private static void SaveAllFiles(string sourcePath, string destinationPath, string saveName)
        {
            LoggerService logger = new LoggerService("logs"); // !!!!!!!!
            logger.logProcessFile(saveName);

            //--- Get number of files in the folder and sub folders
            int fileCount = 0;
            fileCount = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length;

            //--- Get total file size in the folder and sub folders
            long totalFileSize = 0;

            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                totalFileSize += info.Length;
            }

            long fileLeft = fileCount;

            string state = "Starting";

            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft);


            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                DateTime before = DateTime.Now;
                state = "Running";
                logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft);
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                DateTime after = DateTime.Now;
                TimeSpan fileTransferTime;
                fileTransferTime = after - before;
                DateTime time = DateTime.Now;
                int fileSize = newPath.Length;
                logger.logDailySaves(saveName, sourcePath, destinationPath, fileSize, fileTransferTime, time);
                fileLeft--;
            }
            state = "Done";
            fileCount = 0;
            totalFileSize = 0;
            fileLeft = 0;
            sourcePath = "";
            destinationPath = "";
            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft);

        }

        private static void SaveChangedFiles(string sourcePath, string destinationPath, string saveName)
        {

            LoggerService logger = new LoggerService("logs"); // !!!!!!!!
            logger.logProcessFile(saveName);

            //--- Get number of files in the folder and sub folders
            int fileCount = 0;
            fileCount = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length;

            //--- Get total file size in the folder and sub folders
            long totalFileSize = 0;

            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                totalFileSize += info.Length;
            }

            long fileLeft = fileCount;

            string state = "Starting";

            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft);

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

                    DateTime before = DateTime.Now;
                    state = "Running";
                    logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft);
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                    DateTime after = DateTime.Now;
                    TimeSpan fileTransferTime;
                    fileTransferTime = after - before;
                    DateTime time = DateTime.Now;
                    int fileSize = newPath.Length;
                    logger.logDailySaves(saveName, sourcePath, destinationPath, fileSize, fileTransferTime, time);
                }
                else 
                {
                    fileCount--;
                }
                fileLeft--;
            }
            state = "Done";
            fileCount = 0;
            totalFileSize = 0;
            fileLeft = 0;
            sourcePath = "";
            destinationPath = "";
            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft);
        }
    }
}

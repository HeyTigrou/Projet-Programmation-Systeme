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
        
        public static void SaveProcess(SaveWorkModel save) // This method is used to select the right method to use : SaveAllFiles (Complete save) or SaveChangedFiles (Incremental Save)
        {
            if (save.SaveType == 0) // 0 = Complete save
            {
                SaveAllFiles(save.InputPath, save.OutputPath, save.Name); // Launch the complete save
            }
            else if (save.SaveType == 1) // 1 = Incremental Save
            {
                SaveChangedFiles(save.InputPath, save.OutputPath, save.Name); // Launch the incremental save
            }
        }

        private static void SaveAllFiles(string sourcePath, string destinationPath, string saveName) // This method is used to save all the files from the source folder into the destination folder
        {
            LoggerService logger = new LoggerService("logs"); // We create a new logger service to log the process
            logger.logProcessFile(saveName);
 
            int fileCount = 0;
            fileCount = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length; // Get number of files in the folder and sub folders
 
            long totalFileSize = 0;
            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories); // Get total file size in the folder and sub folders
            
            foreach (string file in files) // We're going to collect the size of each file of the directory 
            {
                FileInfo info = new FileInfo(file);
                totalFileSize += info.Length; // Storage of the entire storage 
            }

            long fileLeft = fileCount; // We create a variable to count the number of files left to copy

            string state = "Starting"; // We create a variable to store the state of the process
            string progression = "0%"; // We create a variable to store the progression of the process

            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft, progression); // We log the process state


            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) // We collect all the different folder and sub-foldes
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath)); // We create the same folder and sub-folders in the destination folder
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) // We collect all the files in the folder and sub-folders
            {
                float progress = (fileCount - fileLeft) * 100 / fileCount; // We calculate the progression of the process (%)
                progression = progress.ToString() + "%";
                DateTime before = DateTime.Now; // We get the time before the copy
                state = "Running"; // We change the state of the process
                logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft, progression); // We log the process state

                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true); // We copy the file from the source folder to the destination folder

                DateTime after = DateTime.Now; // We get the time after the copy
                TimeSpan fileTransferTime;
                fileTransferTime = after - before; // We calculate the time of the copy
                DateTime time = DateTime.Now; // We get the time of the copy
                int fileSize = newPath.Length; // We get the size of the file
                logger.logDailySaves(saveName, sourcePath, destinationPath, fileSize, fileTransferTime, time); // We add a log to the daily log file
                fileLeft--;
            }
            state = "Done"; // We change the state of the process : done beacause once we reached this part the process is already done
            fileCount = 0; // We  set all the values to the default value beacause the save process is done
            totalFileSize = 0;
            fileLeft = 0;
            sourcePath = "";
            destinationPath = "";
            progression = "";
            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft, progression); // We log the process state
        }

        private static void SaveChangedFiles(string sourcePath, string destinationPath, string saveName) // This method is used to only save the files that changed in the source folder into the destination folder
        {

            LoggerService logger = new LoggerService("logs"); // We create a new logger service to log the process
            logger.logProcessFile(saveName);

            int fileCount = 0;
            fileCount = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length; // Get number of files in the folder and sub folders

            long totalFileSize = 0;
            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories); // Get total file size in the folder and sub folders

            foreach (string file in files) // We're going to collect the size of each file of the directory
            {
                FileInfo info = new FileInfo(file);
                totalFileSize += info.Length;
            }

            long fileLeft = fileCount;

            string state = "Starting";
            string progression = "0%";

            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft, progression);

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
                    float progress = (fileCount - fileLeft) * 100 / fileCount;
                    progression = progress.ToString() + "%";
                    DateTime before = DateTime.Now;
                    state = "Running";
                    logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft, progression);
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
            progression = "";
            logger.logProcessState(saveName, sourcePath, destinationPath, state, fileCount, totalFileSize, fileLeft, progression);
        }
    }
}

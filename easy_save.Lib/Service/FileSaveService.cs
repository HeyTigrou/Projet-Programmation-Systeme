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
using Newtonsoft.Json.Bson;

namespace easy_save.Lib.Service
{
    public class FileSaveService
    {
        private static StateLoggerModel stateLoggerModel = new StateLoggerModel();

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

        private static void InitializeStateLoggerModel(string sourcePath, string destinationPath, string saveName)
        {
            
            stateLoggerModel.TotalFileToCopy = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length; // Get number of files in the folder and sub folders

            long totalFileSize = 0;
            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories); // Get total file size in the folder and sub folders

            foreach (string file in files) // We're going to collect the size of each file of the directory 
            {
                FileInfo info = new FileInfo(file);
                totalFileSize += info.Length; // Storage of the entire storage 
            }
            stateLoggerModel.TotalFileSize = totalFileSize;

            stateLoggerModel.NbFilesLeft = stateLoggerModel.TotalFileToCopy; // We create a variable to count the number of files left to copy

            stateLoggerModel.State = "Starting"; // We create a variable to store the state of the process
            stateLoggerModel.Progression = "0%"; // We create a variable to store the progression of the process
            stateLoggerModel.SourceFilePath = sourcePath;
            stateLoggerModel.TargetFilePath= destinationPath;
            stateLoggerModel.Name = saveName;
        }

        private static void StateLoggerToNeutral()
        {
            //--- We change all the values to neutral because the process has ended
            stateLoggerModel.State = "Done";
            stateLoggerModel.TotalFileSize = 0;
            stateLoggerModel.NbFilesLeft= 0;
            stateLoggerModel.TotalFileToCopy = 0;
            stateLoggerModel.SourceFilePath = "";
            stateLoggerModel.TargetFilePath = "";
            stateLoggerModel.Progression = "";
        }

        private static void SaveAllFiles(string sourcePath, string destinationPath, string saveName) // This method is used to save all the files from the source folder into the destination folder
        {
            LoggerService logger = new LoggerService("logs"); // We create a new logger service to log the process
            logger.logProcessFile(saveName);

            InitializeStateLoggerModel(sourcePath, destinationPath, saveName);

            logger.logProcessState(stateLoggerModel); // We log the process state


            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) // We collect all the different folder and sub-foldes
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath)); // We create the same folder and sub-folders in the destination folder
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) // We collect all the files in the folder and sub-folders
            {
                long progress = (stateLoggerModel.TotalFileToCopy - stateLoggerModel.NbFilesLeft) * 100 / stateLoggerModel.TotalFileToCopy; // We calculate the progression of the process (%)

                stateLoggerModel.Progression = progress.ToString() + "%";
                DateTime before = DateTime.Now; // We get the time before the copy
                stateLoggerModel.State = "Running"; // We change the state of the process
                logger.logProcessState(stateLoggerModel); // We log the process state

                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true); // We copy the file from the source folder to the destination folder

                DateTime after = DateTime.Now; // We get the time after the copy
                TimeSpan fileTransferTime;
                fileTransferTime = after - before; // We calculate the time of the copy
                DateTime time = DateTime.Now; // We get the time of the copy
                int fileSize = newPath.Length; // We get the size of the file
                logger.logDailySaves(saveName, sourcePath, destinationPath, fileSize, fileTransferTime, time); // We add a log to the daily log file
                stateLoggerModel.NbFilesLeft--;
            }

            StateLoggerToNeutral();

            logger.logProcessState(stateLoggerModel); // We log the process state
        }

        private static void SaveChangedFiles(string sourcePath, string destinationPath, string saveName) // This method is used to only save the files that changed in the source folder into the destination folder
        {

            LoggerService logger = new LoggerService("logs"); // We create a new logger service to log the process
            logger.logProcessFile(saveName);

            InitializeStateLoggerModel(sourcePath, destinationPath, saveName);

            logger.logProcessState(stateLoggerModel);

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
                    long progress = (stateLoggerModel.TotalFileToCopy - stateLoggerModel.NbFilesLeft) * 100 / stateLoggerModel.TotalFileToCopy; // We calculate the progression of the process (%)

                    stateLoggerModel.Progression = progress.ToString() + "%";
                    DateTime before = DateTime.Now; // We get the time before the copy
                    stateLoggerModel.State = "Running"; // We change the state of the process
                    logger.logProcessState(stateLoggerModel); // We log the process state

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
                    stateLoggerModel.TotalFileToCopy--;
                }
                stateLoggerModel.NbFilesLeft--;
            }

            StateLoggerToNeutral();

            logger.logProcessState(stateLoggerModel);
        }
    }
}

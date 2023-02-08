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
        // We create a instance of the logger service that will be used later
        private static StateLoggerModel stateLoggerModel = new();
        
        // This method is used to select the right method to use : SaveAllFiles (Complete save) or SaveChangedFiles (Incremental Save)
        public static void SaveProcess(SaveWorkModel save) 
        {
            // 0 = Complete save
            if (save.SaveType == 0) 
            {
                // Launch the complete save
                SaveAllFiles(save.InputPath, save.OutputPath, save.Name); 
            }
            // 1 = Incremental Save
            else if (save.SaveType == 1) 
            {
                // Launch the incremental save
                SaveChangedFiles(save.InputPath, save.OutputPath, save.Name); 
            }
        }

        // This method is used to initialise the logger
        private static void InitializeStateLoggerModel(string sourcePath, string destinationPath, string saveName)
        {
            // Get number of files in the folder and sub folders
            stateLoggerModel.TotalFileToCopy = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length; 

            long totalFileSize = 0;
            // Get total file size in the folder and sub folders
            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);

            // We're going to collect the size of each file of the directory 
            foreach (string file in files) 
            {
                FileInfo info = new(file);
                // Storage of the entire storage 
                totalFileSize += info.Length; 
            }
            stateLoggerModel.TotalFileSize = totalFileSize;

            // We create a variable to count the number of files left to copy
            stateLoggerModel.NbFilesLeft = stateLoggerModel.TotalFileToCopy;

            // We create a variable to store the state of the process
            stateLoggerModel.State = "Starting";
            // We create a variable to store the progression of the process
            stateLoggerModel.Progression = "0%"; 
            stateLoggerModel.SourceFilePath = sourcePath;
            stateLoggerModel.TargetFilePath= destinationPath;
            stateLoggerModel.Name = saveName;
        }

        // This method is used to reset the state logger once the process is stopped
        private static void StateLoggerToNeutral()
        {
            // We change all the values to neutral because the process has ended
            stateLoggerModel.State = "Done";
            stateLoggerModel.TotalFileSize = 0;
            stateLoggerModel.NbFilesLeft= 0;
            stateLoggerModel.TotalFileToCopy = 0;
            stateLoggerModel.SourceFilePath = "";
            stateLoggerModel.TargetFilePath = "";
            stateLoggerModel.Progression = "";
        }

        // This method is used to save all the files from the source folder into the destination folder
        private static void SaveAllFiles(string sourcePath, string destinationPath, string saveName) 
        {
            // We create a new logger service to log the process
            LoggerService logger = new("logs");
            logger.logProcessFile(saveName);

            InitializeStateLoggerModel(sourcePath, destinationPath, saveName);

            // We log the process state
            logger.logProcessState(stateLoggerModel);

            // We collect all the different folder and sub-foldes
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) 
            {
                // We create the same folder and sub-folders in the destination folder
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath)); 
            }
            // We collect all the files in the folder and sub-folders
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) 
            {
                // We calculate the progression of the process (%)
                long progress = (stateLoggerModel.TotalFileToCopy - stateLoggerModel.NbFilesLeft) * 100 / stateLoggerModel.TotalFileToCopy; 

                stateLoggerModel.Progression = progress.ToString() + "%";
                // We get the time before the copy
                DateTime before = DateTime.Now;
                // We change the state of the process
                stateLoggerModel.State = "Running";
                // We log the process state
                logger.logProcessState(stateLoggerModel);

                // We copy the file from the source folder to the destination folder
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);

                // We get the time after the copy
                DateTime after = DateTime.Now; 
                TimeSpan fileTransferTime;
                // We calculate the time of the copy
                fileTransferTime = after - before;
                // We get the time of the copy
                DateTime time = DateTime.Now;
                // We get the size of the file
                int fileSize = newPath.Length;
                // We add a log to the daily log file
                logger.logDailySaves(saveName, sourcePath, destinationPath, fileSize, fileTransferTime, time); 
                stateLoggerModel.NbFilesLeft--;
            }

            StateLoggerToNeutral();
            // We log the process state
            logger.logProcessState(stateLoggerModel); 
        }

        // This method is used to only save the files that changed in the source folder into the destination folder
        private static void SaveChangedFiles(string sourcePath, string destinationPath, string saveName) 
        {
            // We create a new logger service to log the process
            LoggerService logger = new("logs"); 
            logger.logProcessFile(saveName);

            // We initialise the logger
            InitializeStateLoggerModel(sourcePath, destinationPath, saveName); 

            logger.logProcessState(stateLoggerModel);

            // We creating the directories that aren't in the destination directory
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))  
            {
                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(dirPath);
                DirectoryInfo destinationDirectoryInfo = new DirectoryInfo(dirPath.Replace(sourcePath, destinationPath));
                // If the directory doesn't exist
                if (!destinationDirectoryInfo.Exists)
                {
                    // We create the sub directory
                    destinationDirectoryInfo.Create();
                }
            }
            // We collect all the files in the folder and sub-folders
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {

                FileInfo sourceFileInfo = new(newPath);
                FileInfo destinationFileInfo = new(newPath.Replace(sourcePath, destinationPath));
                // We check if the file in the destination repertory exists or isn't updated
                if (!destinationFileInfo.Exists || sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    // We calculate the progression of the process (%)
                    long progress = (stateLoggerModel.TotalFileToCopy - stateLoggerModel.NbFilesLeft) * 100 / stateLoggerModel.TotalFileToCopy; 

                    stateLoggerModel.Progression = progress.ToString() + "%";
                    // We get the time before the copy
                    DateTime before = DateTime.Now;
                    // We change the state of the process
                    stateLoggerModel.State = "Running";
                    // We log the process state
                    logger.logProcessState(stateLoggerModel); 

                    // We copy the file
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);

                    // We calculate the save time
                    DateTime after = DateTime.Now;
                    TimeSpan fileTransferTime;
                    fileTransferTime = after - before;
                    DateTime time = DateTime.Now;

                    int fileSize = newPath.Length;

                    // We add a log to the daily log file
                    logger.logDailySaves(saveName, sourcePath, destinationPath, fileSize, fileTransferTime, time);
                }
                else 
                {
                    stateLoggerModel.TotalFileToCopy--;
                }
                stateLoggerModel.NbFilesLeft--;
            }

            StateLoggerToNeutral();

            // We update the state log
            logger.logProcessState(stateLoggerModel);
        }
    }
}

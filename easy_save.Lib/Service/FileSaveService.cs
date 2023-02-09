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
        // We create a instance of the logger service
        private readonly StateLoggerModel stateLoggerModel = new ();
        private readonly DailyLoggerModel dailyLoggerModel = new();

        // This method is used to select the right method to use : SaveAllFiles (Complete save) or SaveChangedFiles (Incremental Save)
        public void SaveProcess(SaveWorkModel save) 
        {

            LoggerService logger = new("logs");

            // 0 = Complete save
            if (save.SaveType == 0) 
            {
                // Launch the complete save
                SaveAllFiles(save, logger); 
            }
            // 1 = Incremental Save
            else if (save.SaveType == 1) 
            {
                // Launch the incremental save
                SaveChangedFiles(save, logger); 
            }
        }

        // This method is used to initialise the logger
        private void InitializeLoggerModels(SaveWorkModel save)
        {
            // Get number of files in the folder and sub folders
            

            long totalFileSize = 0;
            // Get total file size in the folder and sub folders
            string[] files = Directory.GetFiles(save.InputPath, "*.*", SearchOption.AllDirectories);
            // Get number of files in the folder and sub folders
            stateLoggerModel.TotalFileToCopy = files.Length;

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
            stateLoggerModel.State = nameof(StateTypes.Running);
            // We create a variable to store the progression of the process 
            stateLoggerModel.SourceFilePath = save.InputPath;
            stateLoggerModel.TargetFilePath= save.OutputPath;
            stateLoggerModel.Name = save.Name;

            dailyLoggerModel.SourceFilePath = save.InputPath;
            dailyLoggerModel.TargetFilePath = save.OutputPath;
            dailyLoggerModel.Name = save.Name;
        }

        // This method is used to reset the state logger once the process is stopped
        private void StateLoggerToDone()
        {
            // We change all the values to neutral because the process has ended
            stateLoggerModel.State = nameof(StateTypes.Done);
            stateLoggerModel.TotalFileSize = 0;
            stateLoggerModel.NbFilesLeft = 0;
            stateLoggerModel.TotalFileToCopy = 0;
            stateLoggerModel.SourceFilePath = "";
            stateLoggerModel.TargetFilePath = "";
        }

        // This method is used to save all the files from the source folder into the destination folder
        private void SaveAllFiles(SaveWorkModel save, LoggerService logger) 
        {
            logger.logProcessFile(save.Name);

            InitializeLoggerModels(save);

            // We log the process state
            logger.logProcessState(stateLoggerModel);

            // We collect all the different folder and sub-foldes
            foreach (string dirPath in Directory.GetDirectories(save.InputPath, "*", SearchOption.AllDirectories)) 
            {
                // We create the same folder and sub-folders in the destination folder
                Directory.CreateDirectory(dirPath.Replace(save.InputPath, save.OutputPath)); 
            }
            // We collect all the files in the folder and sub-folders
            foreach (string newPath in Directory.GetFiles(save.InputPath, "*.*", SearchOption.AllDirectories)) 
            {
                DateTime before = DateTime.Now;

                logger.logProcessState(stateLoggerModel);

                File.Copy(newPath, newPath.Replace(save.InputPath, save.OutputPath), true);

                DateTime after = DateTime.Now;

                FileInfo fileLength = new FileInfo(newPath);
                dailyLoggerModel.Filesize = fileLength.Length;

                dailyLoggerModel.FileTransferTime = after - before;

                dailyLoggerModel.Time = after.ToString("yyyy/MM/dd HH:mm:ss");

                logger.AddToDailyLogJson(dailyLoggerModel);
                stateLoggerModel.NbFilesLeft--;
            }

            StateLoggerToDone();

            logger.logDailySaves();

            logger.logProcessState(stateLoggerModel); 
        }

        // This method is used to only save the files that changed in the source folder into the destination folder
        private void SaveChangedFiles(SaveWorkModel save, LoggerService logger) 
        {
            logger.logProcessFile(save.Name);

            InitializeLoggerModels(save); 

            logger.logProcessState(stateLoggerModel);

            foreach (string dirPath in Directory.GetDirectories(save.InputPath, "*", SearchOption.AllDirectories))  
            {
                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(dirPath);
                DirectoryInfo destinationDirectoryInfo = new DirectoryInfo(dirPath.Replace(save.InputPath, save.OutputPath));
                if (!destinationDirectoryInfo.Exists)
                {
                    destinationDirectoryInfo.Create();
                }
            }
            // We collect all the files in the folder and sub-folders
            foreach (string newPath in Directory.GetFiles(save.InputPath, "*.*", SearchOption.AllDirectories))
            {

                FileInfo sourceFileInfo = new(newPath);
                FileInfo destinationFileInfo = new(newPath.Replace(save.InputPath, save.OutputPath));
                if (!destinationFileInfo.Exists || sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    
                    DateTime before = DateTime.Now;

                    logger.logProcessState(stateLoggerModel); 

                    File.Copy(newPath, newPath.Replace(save.InputPath, save.OutputPath), true);

                    DateTime after = DateTime.Now;


                    FileInfo fileLength = new FileInfo(newPath);
                    dailyLoggerModel.Filesize = fileLength.Length;

                    dailyLoggerModel.FileTransferTime = after - before;

                    dailyLoggerModel.Time = after.ToString("yyyy/MM/dd HH:mm:ss");

                    logger.AddToDailyLogJson(dailyLoggerModel);
                }
                else 
                {
                    stateLoggerModel.TotalFileToCopy--;
                }
                stateLoggerModel.NbFilesLeft--;
            }

            StateLoggerToDone();

            logger.logDailySaves();

            logger.logProcessState(stateLoggerModel);
        }
    }
}

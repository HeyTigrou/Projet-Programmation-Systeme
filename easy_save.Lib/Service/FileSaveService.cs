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
        // We create an instance of each type of logger
        private readonly StateLoggerModel stateLoggerModel = new();

        // This method is used to select the right method to use : SaveAllFiles (Complete save) or SaveChangedFiles (Incremental Save)
        public int SaveProcess(SaveWorkModel save)
        {

            LoggerService logger = new();

            if (save.SaveType == 0)
            {
                return SaveAllFiles(save, logger);
            }

            else if (save.SaveType == 1)
            {
                return SaveChangedFiles(save, logger);
            }

            return -1;
        }

        // This method is used to setup the logger models
        private void InitializeLoggerModels(SaveWorkModel save)
        {
            long totalFileSize = 0;

            string[] files = Directory.GetFiles(save.InputPath, "*.*", SearchOption.AllDirectories);

            stateLoggerModel.TotalFileToCopy = files.Length;

            foreach (string file in files)
            {
                FileInfo info = new(file);
                totalFileSize += info.Length;
            }

            stateLoggerModel.TotalFileSize = totalFileSize;

            stateLoggerModel.NbFilesLeft = stateLoggerModel.TotalFileToCopy;

            stateLoggerModel.State = nameof(StateTypes.Running);
            stateLoggerModel.SourceFilePath = save.InputPath;
            stateLoggerModel.TargetFilePath = save.OutputPath;
            stateLoggerModel.Name = save.Name;
        }

        // This method is used to reset the state logger once the process is stopped
        private void StateLoggerToDone()
        {
            stateLoggerModel.State = nameof(StateTypes.Done);
            stateLoggerModel.TotalFileSize = 0;
            stateLoggerModel.NbFilesLeft = 0;
            stateLoggerModel.TotalFileToCopy = 0;
            stateLoggerModel.SourceFilePath = "";
            stateLoggerModel.TargetFilePath = "";
        }

        // This method is used to save all the files from the source folder into the destination folder
        private int SaveAllFiles(SaveWorkModel save, LoggerService logger)
        {
            logger.logProcessFile(save.Name);

            InitializeLoggerModels(save);

            logger.logProcessState(stateLoggerModel);

            foreach (string dirPath in Directory.GetDirectories(save.InputPath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(save.InputPath, save.OutputPath));
            }

            int errorCount = 0;
            foreach (string newPath in Directory.GetFiles(save.InputPath, "*.*", SearchOption.AllDirectories))
            {
                
                DailyLoggerModel dailyLoggerModel = new();
                dailyLoggerModel.Name = save.Name;

                DateTime before = DateTime.Now;
                
                try
                {
                    logger.logProcessState(stateLoggerModel);
                }
                catch { }

                try
                {
                    File.Copy(newPath, newPath.Replace(save.InputPath, save.OutputPath), true);
                    DateTime after = DateTime.Now;
                    dailyLoggerModel.FileTransferTime = after - before;
                    dailyLoggerModel.Time = after.ToString("yyyy/MM/dd HH:mm:ss");
                }
                catch
                {
                    errorCount++;
                    dailyLoggerModel.FileTransferTime = TimeSpan.Zero;
                    dailyLoggerModel.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                }

                dailyLoggerModel.SourceFilePath = newPath;
                dailyLoggerModel.TargetFilePath = newPath.Replace(save.InputPath, save.OutputPath);
                FileInfo fileLength = new FileInfo(newPath);
                dailyLoggerModel.Filesize = fileLength.Length;

                try
                {
                    logger.AddToDailyLogJson(dailyLoggerModel);
                }
                catch { }

                stateLoggerModel.NbFilesLeft--;
            }

            try
            {
                StateLoggerToDone();

                logger.logDailySaves();

                logger.logProcessState(stateLoggerModel);
            }
            catch { }

            return errorCount;
        }

        // This method is used to only save the files that changed in the source folder into the destination folder
        private int SaveChangedFiles(SaveWorkModel save, LoggerService logger)
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

            int errorCount = 0;
            foreach (string newPath in Directory.GetFiles(save.InputPath, "*.*", SearchOption.AllDirectories))
            {

                FileInfo sourceFileInfo = new(newPath);
                FileInfo destinationFileInfo = new(newPath.Replace(save.InputPath, save.OutputPath));
                if (!destinationFileInfo.Exists || sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    DailyLoggerModel dailyLoggerModel = new();
                    dailyLoggerModel.Name = save.Name;

                    DateTime before = DateTime.Now;

                    try
                    {
                        logger.logProcessState(stateLoggerModel);
                    }
                    catch { }
                    
                    try
                    {
                        File.Copy(newPath, newPath.Replace(save.InputPath, save.OutputPath), true);
                        DateTime after = DateTime.Now;
                        dailyLoggerModel.FileTransferTime = after - before;
                        dailyLoggerModel.Time = after.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    catch
                    {
                        errorCount++;
                        dailyLoggerModel.FileTransferTime = TimeSpan.Zero;
                        dailyLoggerModel.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    }

                    dailyLoggerModel.SourceFilePath = newPath;
                    dailyLoggerModel.TargetFilePath = newPath.Replace(save.InputPath, save.OutputPath);
                    FileInfo fileLength = new FileInfo(newPath);
                    dailyLoggerModel.Filesize = fileLength.Length;

                    try
                    {
                        logger.AddToDailyLogJson(dailyLoggerModel);
                    }
                    catch { }
                }
                else
                {
                    stateLoggerModel.TotalFileToCopy--;
                }
                stateLoggerModel.NbFilesLeft--;
            }

            try
            {
                StateLoggerToDone();

                logger.logDailySaves();

                logger.logProcessState(stateLoggerModel);
            }
            catch { }
            
            return errorCount;
        }
    }
}
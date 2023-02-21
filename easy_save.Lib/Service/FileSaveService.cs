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
using System.Diagnostics;
using System.Configuration;

namespace easy_save.Lib.Service
{
    public class FileSaveService
    {
        /// <summary>
        /// We create an instance of each type of logger
        /// </summary>
        public readonly StateLoggerModel stateLoggerModel = new();

        public event EventHandler<SaveWorkModel> ThreadEnded;
        public SaveWorkModel Save;

        public bool QuitThread { get; set; } = false;
        public ManualResetEvent ResetEvent = new ManualResetEvent(false);

        /// <summary>
        /// This method is used to select the right method to use : SaveAllFiles (Complete save) or SaveChangedFiles (Incremental Save)
        /// </summary>
        /// <param name="save"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public void SaveProcess(SaveWorkModel save, List<string> extensions)
        {
            new Thread(() =>
            {
                Save = save;
                Save.State = nameof(SaveWorkState.Running);
                ResetEvent.Set();
                LoggerService logger = new();
                // Checks save type and launchs the corresponding method.
                if (Save.SaveType == 0)
                {
                     SaveAllFiles(logger, extensions);
                }

                else if (Save.SaveType == 1)
                {
                     SaveChangedFiles(logger, extensions);
                }
                Save.State = nameof(SaveWorkState.Done);
                ThreadEnded?.Invoke(this, Save);
            }).Start();
        }

        public void Pause()
        {
            Save.State = nameof(SaveWorkState.Paused);
            ResetEvent.Reset();
        }
        public void Resume()
        {
            Save.State = nameof(SaveWorkState.Running);
            ResetEvent.Set();
        }

        public void Quit()
        {
            Save.State = nameof(SaveWorkState.Done);
            QuitThread = true;
            ResetEvent.Set();
        }

        /// <summary>
        /// This method is used to setup the logger models
        /// </summary>
        /// <param name="save"></param>
        private void InitializeLoggerModels()
        {
            long totalFileSize = 0;

            
            string[] files = Directory.GetFiles(Save.InputPath, "*.*", SearchOption.AllDirectories);

            stateLoggerModel.TotalFileToCopy = files.Length;

            // Gets the total size of files.
            foreach (string file in files)
            {
                FileInfo info = new(file);
                totalFileSize += info.Length;
            }

            stateLoggerModel.TotalFileSize = totalFileSize;

            stateLoggerModel.NbFilesLeft = stateLoggerModel.TotalFileToCopy;

            stateLoggerModel.State = nameof(SaveWorkState.Running);
            stateLoggerModel.SourceFilePath = Save.InputPath;
            stateLoggerModel.TargetFilePath = Save.OutputPath;
            stateLoggerModel.Name = Save.Name;
        }

        /// <summary>
        /// This method is used to reset the state logger once the process is stopped
        /// </summary>
        private void StateLoggerToDone()
        {
            stateLoggerModel.State = nameof(SaveWorkState.Done);
            stateLoggerModel.TotalFileSize = 0;
            stateLoggerModel.NbFilesLeft = 0;
            stateLoggerModel.TotalFileToCopy = 0;
            stateLoggerModel.SourceFilePath = "";
            stateLoggerModel.TargetFilePath = "";
        }

        /// <summary>
        /// This method is used to save all the files from the source folder into the destination folder
        /// </summary>
        /// <param name="save"></param>
        /// <param name="logger"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private int SaveAllFiles(LoggerService logger, List<string> extensions)
        {
            logger.LogProcessFile(Save.Name);

            InitializeLoggerModels();

            logger.LogProcessState(stateLoggerModel);

            // Creates directories in the destination directory to reproduce the architecture of the source directory.
            foreach (string dirPath in Directory.GetDirectories(Save.InputPath, "*", SearchOption.AllDirectories))
            {
                ResetEvent.WaitOne();
                if(QuitThread == true)
                {
                    return 10;
                }

                Directory.CreateDirectory(dirPath.Replace(Save.InputPath, Save.OutputPath));
            }

            int errorCount = 0;
            // Copies each file to the destionation path.
            foreach (string newPath in Directory.GetFiles(Save.InputPath, "*.*", SearchOption.AllDirectories))
            {
                ResetEvent.WaitOne();
                if (QuitThread == true)
                {
                    return 10;
                }

                DailyLoggerModel dailyLoggerModel = new();
                dailyLoggerModel.Name = Save.Name;

                DateTime before = DateTime.Now;
                
                logger.LogProcessState(stateLoggerModel);

                try
                {
                    int returnCode = CopyProcess(newPath, extensions);
                    
                    DateTime after = DateTime.Now;
                    dailyLoggerModel.FileTransferTime = after - before;
                    dailyLoggerModel.Time = after.ToString("yyyy/MM/dd HH:mm:ss");
                    dailyLoggerModel.CryptTime = returnCode;
                }
                catch
                {
                    errorCount++;
                    dailyLoggerModel.FileTransferTime = TimeSpan.Zero;
                    dailyLoggerModel.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    dailyLoggerModel.CryptTime = -1;
                }

                dailyLoggerModel.SourceFilePath = newPath;
                dailyLoggerModel.TargetFilePath = newPath.Replace(Save.InputPath, Save.OutputPath);
                FileInfo fileLength = new FileInfo(newPath);
                dailyLoggerModel.Filesize = fileLength.Length;

                logger.AddToDailyLog(dailyLoggerModel);

                stateLoggerModel.NbFilesLeft--;
                Save.Progression = (stateLoggerModel.Progression * 100).ToString("0.0") + "%";
            }

            StateLoggerToDone();

            logger.LogDailySaves();

            logger.LogProcessState(stateLoggerModel);

            return errorCount;
        }

        /// <summary>
        /// This method is used to only save the files that changed in the source folder into the destination folder
        /// </summary>
        /// <param name="save"></param>
        /// <param name="logger"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private int SaveChangedFiles(LoggerService logger, List<string> extensions)
        {
            logger.LogProcessFile(Save.Name);

            InitializeLoggerModels();

            logger.LogProcessState(stateLoggerModel);

            // Creates directories in the destination directory to reproduce the architecture of the source directory, if it does not exist.
            foreach (string dirPath in Directory.GetDirectories(Save.InputPath, "*", SearchOption.AllDirectories))
            {
                ResetEvent.WaitOne();
                if (QuitThread == true)
                {
                    return 10;
                }

                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(dirPath);
                DirectoryInfo destinationDirectoryInfo = new DirectoryInfo(dirPath.Replace(Save.InputPath, Save.OutputPath));
                if (!destinationDirectoryInfo.Exists)
                {
                    destinationDirectoryInfo.Create();
                }
            }

            int errorCount = 0;
            // Copies modified files to the destionation path.
            foreach (string newPath in Directory.GetFiles(Save.InputPath, "*.*", SearchOption.AllDirectories))
            {
                ResetEvent.WaitOne();
                if (QuitThread == true)
                {
                    return 10;
                }

                FileInfo sourceFileInfo = new(newPath);
                FileInfo destinationFileInfo = new(newPath.Replace(Save.InputPath, Save.OutputPath));
                if (!destinationFileInfo.Exists || sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    DailyLoggerModel dailyLoggerModel = new();
                    dailyLoggerModel.Name = Save.Name;

                    DateTime before = DateTime.Now;

                    logger.LogProcessState(stateLoggerModel);
                    

                    try
                    {
                        int returnCode = CopyProcess(newPath, extensions);

                        DateTime after = DateTime.Now;
                        dailyLoggerModel.FileTransferTime = after - before;
                        dailyLoggerModel.Time = after.ToString("yyyy/MM/dd HH:mm:ss");
                        dailyLoggerModel.CryptTime = returnCode;
                    }
                    catch
                    {
                        errorCount++;
                        dailyLoggerModel.FileTransferTime = TimeSpan.Zero;
                        dailyLoggerModel.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        dailyLoggerModel.CryptTime = -1;
                    }

                    dailyLoggerModel.SourceFilePath = newPath;
                    dailyLoggerModel.TargetFilePath = newPath.Replace(Save.InputPath, Save.OutputPath);
                    FileInfo fileLength = new FileInfo(newPath);
                    dailyLoggerModel.Filesize = fileLength.Length;

                    logger.AddToDailyLog(dailyLoggerModel);
                }
                else
                {
                    stateLoggerModel.TotalFileToCopy--;
                }
                stateLoggerModel.NbFilesLeft--;
                Save.Progression = (stateLoggerModel.Progression * 100).ToString("0.0") + "%";
            }

            StateLoggerToDone();
                    
            logger.LogDailySaves();

            logger.LogProcessState(stateLoggerModel);

            return errorCount;
        }

        /// <summary>
        /// This method is used in both saves processes to crypt and comy the file.
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="save"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private int CopyProcess(string inPath,List<string> extensions)
        {
            FileInfo fileInfo = new FileInfo(inPath);

            // Creates the destination path, by keeping the file name and changing the folder path.
            string destinationPath = inPath.Replace(Save.InputPath, Save.OutputPath);
            int returnCode = 0;

            // If the extension list is not empty
            if (extensions.Contains(fileInfo.Extension))
            {
                // Crypts the files with the selected extensions.
                CryptService crypt = new CryptService();
                returnCode = crypt.Crypt(inPath, destinationPath);
            }

            else
            {
                // If the list is empty copies the file without encrypting them.    
                File.Copy(inPath, destinationPath, true);
            }

            return returnCode;
        }
    }
}
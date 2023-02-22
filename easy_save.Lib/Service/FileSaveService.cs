using easy_save.Lib.Models;
using DetectSoftware;
using System.Configuration;
using System.Collections.Generic;

namespace easy_save.Lib.Service
{
    public class FileSaveService
    {
        /// <summary>
        /// We create an instance of each type of logger
        /// </summary>
        public readonly StateLoggerModel stateLoggerModel = new();
        private static Mutex Mutex = new Mutex(false);

        public event EventHandler<SaveWorkModel> ThreadEnded;
        public SaveWorkModel Save;

        private LoggerService Logger = new LoggerService();

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

                StartSaveProcess(extensions);
                
                Save.State = nameof(SaveWorkState.Done);
                ThreadEnded?.Invoke(this, Save);
            }).Start();
            new Thread(() =>
            {
                ProcessStateService service = new ProcessStateService();
                while (Save.State != nameof(SaveWorkState.Done))
                {
                    if(service.GetProcessState(ConfigurationManager.AppSettings["WorkProcessName"]) && Save.State == nameof(SaveWorkState.Running))
                    {
                        ResetEvent.Reset();
                        Save.State = nameof(SaveWorkState.Paused_ApplicationIsRunning);
                    }
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        private void StartSaveProcess(List<string> extensions)
        {
            Logger.LogProcessFile(Save.Name);

            InitializeLoggerModels();

            Logger.LogProcessState(stateLoggerModel);

            // Creates directories in the destination directory to reproduce the architecture of the source directory, if it does not exist.
            foreach (string dirPath in Directory.GetDirectories(Save.InputPath, "*", SearchOption.AllDirectories))
            {
                ResetEvent.WaitOne();
                if (QuitThread == true)
                {
                    return;
                }

                DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(dirPath);
                DirectoryInfo destinationDirectoryInfo = new DirectoryInfo(dirPath.Replace(Save.InputPath, Save.OutputPath));
                if (!destinationDirectoryInfo.Exists)
                {
                    destinationDirectoryInfo.Create();
                }
            }

            List<List<string>> Files = PriorityProcess(Save.InputPath);

            bool priorityExtansions = false;
            bool largePriorityFiles = false;
            bool normalFiles = false;
            bool largeNormalFiles = false;
            // Checks save type and launchs the corresponding method.
            if (Save.SaveType == 0)
            {
                while (!largePriorityFiles || !priorityExtansions || !largeNormalFiles || !normalFiles)
                {
                    if (!largePriorityFiles)
                    {
                        if (!Files[1].Any())
                            largePriorityFiles = true;

                        if (Mutex.WaitOne(100))
                        {
                            SaveAllFiles(extensions, Files[1]);
                            largePriorityFiles = true;
                            Mutex.ReleaseMutex();
                        }
                    }
                    if (!priorityExtansions)
                    {
                        SaveAllFiles(extensions, Files[0]);
                        priorityExtansions = true;
                    }
                    if (!largeNormalFiles && largePriorityFiles && priorityExtansions)
                    {
                        if (!Files[3].Any())
                            largeNormalFiles = true;

                        if (Mutex.WaitOne(100))
                        {
                            SaveAllFiles(extensions, Files[3]);
                            largeNormalFiles = true;
                            Mutex.ReleaseMutex();
                        }
                    }
                    if (!normalFiles && largePriorityFiles && priorityExtansions)
                    {
                        SaveAllFiles(extensions, Files[2]);
                        normalFiles = true;
                    }
                }
            }
            else if (Save.SaveType == 1)
            {
                while(!largePriorityFiles || !priorityExtansions || !largeNormalFiles || !normalFiles)
                {
                    ResetEvent.WaitOne();
                    if (QuitThread == true)
                    {
                        break;
                    }
                    if (!largePriorityFiles)
                    {
                        if (!Files[1].Any())
                            largePriorityFiles = true;

                        if (Mutex.WaitOne(100))
                        {
                            SaveChangedFiles(extensions, Files[1]);
                            largePriorityFiles= true;
                            Mutex.ReleaseMutex();
                        }
                    }
                    if (!priorityExtansions)
                    {
                        SaveChangedFiles(extensions, Files[0]);
                        priorityExtansions = true;
                    }
                    if (!largeNormalFiles && largePriorityFiles && priorityExtansions)
                    {
                        if (!Files[3].Any())
                            largeNormalFiles = true;

                        if (Mutex.WaitOne(100))
                        {
                            SaveChangedFiles(extensions, Files[3]);
                            largeNormalFiles= true;
                            Mutex.ReleaseMutex();
                        }
                    }
                    if (!normalFiles && largePriorityFiles && priorityExtansions)
                    {
                        SaveChangedFiles(extensions, Files[2]);
                        normalFiles= true;
                    }
                }
            }

            StateLoggerToDone();

            Logger.LogDailySaves();

            Logger.LogProcessState(stateLoggerModel);
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
        private void SaveAllFiles(List<string> extensions, List<string> Files)
        {
            // Copies each file to the destionation path.
            foreach (string newPath in Files)
            {
                ResetEvent.WaitOne();
                if (QuitThread == true)
                {
                    return;
                }

                DailyLoggerModel dailyLoggerModel = new();
                dailyLoggerModel.Name = Save.Name;

                DateTime before = DateTime.Now;
                
                Logger.LogProcessState(stateLoggerModel);

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
                    dailyLoggerModel.FileTransferTime = TimeSpan.Zero;
                    dailyLoggerModel.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    dailyLoggerModel.CryptTime = -1;
                }

                dailyLoggerModel.SourceFilePath = newPath;
                dailyLoggerModel.TargetFilePath = newPath.Replace(Save.InputPath, Save.OutputPath);
                FileInfo fileLength = new FileInfo(newPath);
                dailyLoggerModel.Filesize = fileLength.Length;

                Logger.AddToDailyLog(dailyLoggerModel);

                stateLoggerModel.NbFilesLeft--;
                Save.Progression = (stateLoggerModel.Progression * 100).ToString("0.0") + "%";
            }
        }

        /// <summary>
        /// This method is used to only save the files that changed in the source folder into the destination folder
        /// </summary>
        /// <param name="save"></param>
        /// <param name="logger"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private void SaveChangedFiles(List<string> extensions, List<string> Files)
        {
            // Copies modified files to the destionation path.
            foreach (string newPath in Files)
            {
                ResetEvent.WaitOne();
                if (QuitThread == true)
                {
                    return;
                }

                FileInfo sourceFileInfo = new(newPath);
                FileInfo destinationFileInfo = new(newPath.Replace(Save.InputPath, Save.OutputPath));
                if (!destinationFileInfo.Exists || sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime)
                {
                    DailyLoggerModel dailyLoggerModel = new();
                    dailyLoggerModel.Name = Save.Name;

                    DateTime before = DateTime.Now;

                    Logger.LogProcessState(stateLoggerModel);
                    

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
                        dailyLoggerModel.FileTransferTime = TimeSpan.Zero;
                        dailyLoggerModel.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        dailyLoggerModel.CryptTime = -1;
                    }

                    dailyLoggerModel.SourceFilePath = newPath;
                    dailyLoggerModel.TargetFilePath = newPath.Replace(Save.InputPath, Save.OutputPath);
                    FileInfo fileLength = new FileInfo(newPath);
                    dailyLoggerModel.Filesize = fileLength.Length;

                    Logger.AddToDailyLog(dailyLoggerModel);
                }
                else
                {
                    stateLoggerModel.TotalFileToCopy--;
                }
                stateLoggerModel.NbFilesLeft--;
                Save.Progression = (stateLoggerModel.Progression * 100).ToString("0.0") + "%";
            }
        }

        /// <summary>
        /// This method is used in both saves processes to crypt and comy the file.
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="save"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private int CopyProcess(string inPath, List<string> extensions)
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

        private List<List<string>> PriorityProcess(string inputPath)
        {
            List<string> priorityFiles = new();
            List<string> largePriorityFiles = new();
            List<string> largeNormalFiles = new();
            List<string> normalFiles = new();

            List<string> priorityExtansions = FileExtensionModel.PriorityInstance.SelectedPriorityExtensions.ToList();

            foreach (string FilePath in Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories))
            {
                FileInfo info = new(FilePath);
                if((Int32.Parse(ConfigurationManager.AppSettings["FileSizeLimit"]) < info.Length) && priorityExtansions.Contains(info.Extension))
                {
                    largePriorityFiles.Add(FilePath);
                }
                else if (priorityExtansions.Contains(info.Extension))
                {
                    priorityFiles.Add(FilePath);
                }
                else if (Int32.Parse(ConfigurationManager.AppSettings["FileSizeLimit"]) < info.Length)
                {
                     largeNormalFiles.Add(FilePath);
                }
                else
                {
                     normalFiles.Add(FilePath);
                }
            }

            List<List<string>> Files = new List<List<string>>
            {
                priorityFiles,
                largePriorityFiles,
                normalFiles,
                largeNormalFiles,
            };

            return Files;
        }
    }
}
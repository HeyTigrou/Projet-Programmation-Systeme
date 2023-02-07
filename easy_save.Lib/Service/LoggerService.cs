using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Enumeration;
using easy_save.Lib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Xml;

namespace easy_save.Lib.Service
{
    public class DailyLoggerService
    {
        public string Name { set; get; }
        public string SourceFilePath { set; get; }
        public string TargetFilePath { set; get; }
        public int Filesize { set; get; }
        public TimeSpan FileTransferTime { set; get; }
        public string Time { set; get; }
    }

    public class StateLoggerService
    {
        public string Name { set; get; }
        public string SourceFilePath { set; get; }
        public string TargetFilePath { set; get; }
        public string State { set; get; }
        public int TotalFileToCopy { set; get; }
        public int TotalFileSize { set; get; }
        public int NbFilesLeft { set; get; }
    }

    public class LoggerService
    {
        private string FileName;
        private DateTime Date;
        private string stateLogDirectoryPath;
        private string DailyLogFile;
        private string stateLogFile;
        private string DirectoryPath;

        public LoggerService(string fileName)
        {
            var config = JsonConvert.DeserializeObject<ConfigFileModel>(File.ReadAllText(@"..\..\..\..\easy_save.Lib\ConfigurationFiles\easy_save_config.json"));
            this.FileName = fileName;
            this.DirectoryPath = config.Daily_log_emplacement;
            setLogFilePath();
        }

        private void setLogFilePath()
        {
            Date = DateTime.Today;
            string currentDate = Date.ToString("yyyy-MM-dd");
            this.stateLogDirectoryPath = DirectoryPath + currentDate + "-" + FileName;
            this.DailyLogFile = DirectoryPath + currentDate + "-" + FileName + ".log";
            if (!Directory.Exists(this.stateLogDirectoryPath))
            {
                Directory.CreateDirectory(this.stateLogDirectoryPath);
            }
            if (!File.Exists(this.DailyLogFile))
            {
                File.Create(this.DailyLogFile).Close();
            }
        }

        private void checkDate()
        {
            if (Date != DateTime.Today)
            {
                setLogFilePath();
            }
        }

        public void logProcessFileCreation(string processName)
        {
            stateLogFile = stateLogDirectoryPath + "\\" + processName + ".log";
            File.Create(stateLogFile).Close();
        }

        public void logProcessState(string name, string sourceFilePath, string targetFilePath, string state, int totalFileToCopy, int totalFileSize, int nbFilesLeft)

        {
            string json;
            checkDate();

            StateLoggerService data = new StateLoggerService
            {
                Name = name,
                SourceFilePath = sourceFilePath,
                TargetFilePath = targetFilePath,
                State = state,
                TotalFileToCopy = totalFileToCopy,
                TotalFileSize = totalFileSize,
                NbFilesLeft = nbFilesLeft
            };

            json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = File.AppendText(this.stateLogFile))

            {
                if (new FileInfo(this.stateLogFile).Length != 0)
                {
                    sw.WriteLine(",");
                }
                sw.Write(json);
            }
        }

        public void logDailySaves(string name, string sourceFilePath, string targetFilePath, int filesize, TimeSpan fileTransferTime, DateTime time)
        {
            string json;
            checkDate();

            DailyLoggerService data = new DailyLoggerService
            {
                Name = name,
                SourceFilePath = sourceFilePath,
                TargetFilePath = targetFilePath,
                Filesize = filesize,
                FileTransferTime = fileTransferTime,
                Time = time.ToString("yyyy/MM/dd HH:mm:ss")
            };
            json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = File.AppendText(this.DailyLogFile))
            {
                if (new FileInfo(this.DailyLogFile).Length != 0)
                {

                    sw.WriteLine(",");
                }
                sw.Write(json);
            }
        }
    }
}
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
        public long TotalFileSize { set; get; }
        public long NbFilesLeft { set; get; }
        public string Progression { set; get; }
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
            this.stateLogDirectoryPath = config.Status_log_emplacement;
            setLogFilePath();
        }

        private void setLogFilePath()
        {
            Date = DateTime.Today;
            string currentDate = Date.ToString("yyyy-MM-dd");
            this.DailyLogFile = DirectoryPath + currentDate + "-" + FileName + ".log";
            if (!File.Exists(this.DailyLogFile))
            {
                File.Create(this.DailyLogFile).Close();
            }
        }

        public void logProcessFile(string processName)
        {
            stateLogFile = stateLogDirectoryPath + "\\" + processName + ".log";
            if (!File.Exists(this.stateLogFile))
            {
                File.Create(this.stateLogFile).Close();
            }
        }

        public void logProcessState(string name, string sourceFilePath, string targetFilePath, string state, int totalFileToCopy, long totalFileSize, long nbFilesLeft, string progression)

        {
            string json;
            setLogFilePath();

            StateLoggerService data = new StateLoggerService
            {
                Name = name,
                SourceFilePath = sourceFilePath,
                TargetFilePath = targetFilePath,
                State = state,
                TotalFileToCopy = totalFileToCopy,
                TotalFileSize = totalFileSize,
                NbFilesLeft = nbFilesLeft,
                Progression = progression
            };

            json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(this.stateLogFile, json);

           
        }

        public void logDailySaves(string name, string sourceFilePath, string targetFilePath, int filesize, TimeSpan fileTransferTime, DateTime time)
        {
            string json;
            setLogFilePath();

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
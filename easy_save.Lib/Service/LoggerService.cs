using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Enumeration;
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
        public DateTime FileTransferTime { set; get; }
        public DateTime Time { set; get; }
    }

    public class RealtimeLoggerService
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
        private string RealtimeLogFilePath;
        private string DailyLogFilePath;
        private string DirectoryPath;

        private void setLogFilePath()
        {
            Date = DateTime.Today;
            string currentDate = Date.ToString("yyyy-MM-dd");
            this.RealtimeLogFilePath = DirectoryPath + currentDate + "-" + FileName + ".log";
            this.DailyLogFilePath = DirectoryPath + FileName + ".log";
            if (!File.Exists(this.RealtimeLogFilePath))
            {
                File.Create(this.RealtimeLogFilePath).Close();
            }
            if (!File.Exists(this.DailyLogFilePath))
            {
                File.Create(this.DailyLogFilePath).Close();
            }
        }

        private void checkDate()
        {
            if (Date != DateTime.Today)
            {
                setLogFilePath();
            }
        }

        public LoggerService(string fileName, string directoryPath)
        {
            this.FileName = fileName;
            this.DirectoryPath = directoryPath;
            setLogFilePath();
        }
        
        public void logSaveFolderFilesProgression(string name, string sourceFilePath, string targetFilePath, string state, int totalFileToCopy, int totalFileSize, int nbFilesLeft)

        {
            string json;
            checkDate();

            RealtimeLoggerService data = new RealtimeLoggerService
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
            using (StreamWriter sw = File.AppendText(this.RealtimeLogFilePath))

            {
                if (new FileInfo(this.RealtimeLogFilePath).Length != 0)
                {
                    sw.WriteLine(",");
                }
                sw.Write(json);
            }
        }

        public void logSaveFolderFiles(string name, string sourceFilePath, string targetFilePath, int filesize, DateTime FileTransferTime, DateTime time)
        {
            string json;
            checkDate();

            DailyLoggerService data = new DailyLoggerService
            {
                Name = name,
                SourceFilePath = sourceFilePath,
                TargetFilePath = targetFilePath,
                Filesize = filesize,
                FileTransferTime = FileTransferTime,
                Time = time
            };
            json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = File.AppendText(this.DailyLogFilePath))
            {
                if (new FileInfo(this.DailyLogFilePath).Length != 0)
                {

                    sw.WriteLine(",");
                }
                sw.Write(json);
            }
        }
    }
}

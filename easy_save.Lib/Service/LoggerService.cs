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
    public class LoggerService
    {
        private string FileName;
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
            DateTime Date = DateTime.Today;
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

        public void logProcessState(StateLoggerModel stateLoggerModel)

        {
            string json;
            setLogFilePath();

            json = JsonConvert.SerializeObject(stateLoggerModel, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(this.stateLogFile, json);

           
        }

        public void logDailySaves(string name, string sourceFilePath, string targetFilePath, int filesize, TimeSpan fileTransferTime, DateTime time)
        {
            string json;
            setLogFilePath();

            DailyLoggerModel data = new DailyLoggerModel
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
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
    // This class is used to log the daily saves and the state of the save works
    public class LoggerService
    {
        // The different properties that are used to create the log files
        private string FileName;
        private string stateLogDirectoryPath;
        private string DailyLogFile;
        private string stateLogFile;
        private string DirectoryPath;

        // The constructor of the class
        public LoggerService(string fileName)
        {
            // We get the JSON config files from the configuration file and we assign the properties
            var config = JsonConvert.DeserializeObject<ConfigFileModel>(File.ReadAllText(@"..\..\easy_save.Cmd\ConfigurationFiles\easy_save_config.json"));
            this.FileName = fileName;
            this.DirectoryPath = config.Daily_log_emplacement;
            this.stateLogDirectoryPath = config.Status_log_emplacement;

            // We create the daily log file if it doesn't exist
            SetLogFilePath();
        }

        // This method is used to create the daily log file
        private void SetLogFilePath()
        {
            DateTime date = DateTime.Today;
            string currentDate = date.ToString("yyyy-MM-dd");
            this.DailyLogFile = DirectoryPath + currentDate + "-" + FileName + ".log";
            // If the file doesn't exist, we create it
            if (!File.Exists(this.DailyLogFile))
            {
                File.Create(this.DailyLogFile).Close();
            }
        }

        // This method is used to create the log file that save the state of the save works
        public void logProcessFile(string processName)
        {
            stateLogFile = stateLogDirectoryPath + "\\" + processName + ".log";
            // If the file doesn't exist, we create it
            if (!File.Exists(this.stateLogFile))
            {
                File.Create(this.stateLogFile).Close();
            }
        }

        // This method is used to log the state of the save works
        public void logProcessState(StateLoggerModel stateLoggerModel)

        {
            string json;
            // We call the method that will create the log file if it doesn't exist
            SetLogFilePath();

            // We serialize the object and we write it in the state log file
            json = JsonConvert.SerializeObject(stateLoggerModel, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(this.stateLogFile, json);

           
        }

        // This method is used to log the daily saves
        public void logDailySaves(string name, string sourceFilePath, string targetFilePath, int filesize, TimeSpan fileTransferTime, DateTime time)
        {
            string json;
            // We call the method that will create the log file if it doesn't exist
            SetLogFilePath();

            // We serialize the object and we write it in the daily log file
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
            // We write the object in the daily log file
            using (StreamWriter sw = File.AppendText(this.DailyLogFile))
            {
                // If the file is not empty, we add a comma to separate the objects
                if (new FileInfo(this.DailyLogFile).Length != 0)
                {
                    sw.WriteLine(",");
                }
                sw.Write(json);
            }
        }
    }
}
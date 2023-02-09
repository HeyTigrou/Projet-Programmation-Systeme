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
using System.Xml.Linq;

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
        private List<DailyLoggerModel> DailyLogs = new();

        // The constructor of the class
        public LoggerService(string fileName)
        {
            // We get the JSON config files from the configuration file and we assign the properties
            var config = JsonConvert.DeserializeObject<ConfigFileModel>(File.ReadAllText(@"..\..\easy_save.Cmd\ConfigurationFiles\easy_save_config.json"));
            this.FileName = fileName;
            this.DirectoryPath = config.Daily_log_emplacement;
            this.stateLogDirectoryPath = config.Status_log_emplacement;

        }

        // This method is used to create the daily log file
        private void SetLogFilePath()
        {
            DateTime date = DateTime.Today;
            string currentDate = date.ToString("yyyy-MM-dd");
            this.DailyLogFile = Path.Combine(DirectoryPath + $"{currentDate}-{FileName}.json");
            // If the file doesn't exist, we create it
            if (!File.Exists(this.DailyLogFile))
            {
                File.Create(this.DailyLogFile).Close();
            }
        }

        // This method is used to create the log file that save the state of the save works
        public void logProcessFile(string processName)
        {
            stateLogFile = Path.Combine(stateLogDirectoryPath + $"{processName}.json");
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

            json = System.Text.Json.JsonSerializer.Serialize<StateLoggerModel>(stateLoggerModel);
            // We serialize the object and we write it in the state log file
            File.WriteAllText(this.stateLogFile, json);
        }


        public void AddToDailyLogJson(DailyLoggerModel dailyLoggerModel)
        {
            DailyLogs.Add(dailyLoggerModel);
        }

        // This method is used to log the daily saves
        public void logDailySaves()
        {
            // We call the method that will create the log file if it doesn't exist
            SetLogFilePath();
            string DailyLogString;
            string filetext;

            List<DailyLoggerModel> fileData = new List<DailyLoggerModel>();
            if (new FileInfo(DailyLogFile).Length != 0)
            {
                filetext = File.ReadAllText(DailyLogFile);
                fileData = JsonConvert.DeserializeObject<List<DailyLoggerModel>>(filetext);
            }
            fileData.AddRange(DailyLogs);

            DailyLogString = System.Text.Json.JsonSerializer.Serialize(fileData);

            File.WriteAllText(DailyLogFile, DailyLogString);

        }
    }
}
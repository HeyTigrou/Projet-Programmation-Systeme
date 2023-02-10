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
using System.Configuration;

namespace easy_save.Lib.Service
{
    // This class is used to log the daily saves and the state of the save works
    public class LoggerService
    {
        // The different properties that are used to create the log files
        private string FileName;
        private string StateLogDirectoryPath;
        private string DailyLogFile;
        private string StateLogFile;
        private string DirectoryPath;
        private List<DailyLoggerModel> DailyLogs = new();

        // The constructor of the class
        public LoggerService()
        {
            // We get the JSON config files from the configuration file and we assign the properties

            //var config = JsonConvert.DeserializeObject<ConfigFileModel>(File.ReadAllText(@"..\..\..\ConfigurationFiles\easy_save_config.json"));
            FileName = ConfigurationManager.AppSettings["LogFileName"];
            DirectoryPath = $@"{ConfigurationManager.AppSettings["DailyLogEmplacement"]}";
            StateLogDirectoryPath = $@"{ConfigurationManager.AppSettings["StatusLogEmplacement"]}";
            if(!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            if(!Directory.Exists(StateLogDirectoryPath))
            {
                Directory.CreateDirectory(StateLogDirectoryPath);
            }
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
            StateLogFile = Path.Combine(StateLogDirectoryPath + $"{processName}.json");
            // If the file doesn't exist, we create it
            if (!File.Exists(this.StateLogFile))
            {
                File.Create(this.StateLogFile).Close();
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
            File.WriteAllText(this.StateLogFile, json);
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
                fileData = System.Text.Json.JsonSerializer.Deserialize<List<DailyLoggerModel>>(filetext);
            }
            fileData.AddRange(DailyLogs);

            DailyLogString = System.Text.Json.JsonSerializer.Serialize(fileData);

            File.WriteAllText(DailyLogFile, DailyLogString);

        }

    }
}
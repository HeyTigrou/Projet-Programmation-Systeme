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
        private string FileName;
        private string StateLogDirectoryPath;
        private string DailyLogFile;
        private string StateLogFile;
        private string DirectoryPath;
        private List<DailyLoggerModel> DailyLogs = new();

        // The constructor of the class
        public LoggerService()
        {
            FileName = ConfigurationManager.AppSettings["LogFileName"];
            DirectoryPath = $@"{ConfigurationManager.AppSettings["DailyLogEmplacement"]}";
            StateLogDirectoryPath = $@"{ConfigurationManager.AppSettings["StatusLogEmplacement"]}";

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            if (!Directory.Exists(StateLogDirectoryPath))
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

            if (!File.Exists(this.DailyLogFile))
            {
                File.Create(this.DailyLogFile).Close();
            }
        }

        // This method is used to create the log file that save the state of the save works
        public void logProcessFile(string processName)
        {
            StateLogFile = Path.Combine(StateLogDirectoryPath + $"{processName}.json");

            if (!File.Exists(this.StateLogFile))
            {
                File.Create(this.StateLogFile).Close();
            }
        }

        // This method is used to log the state of the save works
        public void logProcessState(StateLoggerModel stateLoggerModel)
        {
            string json;

            SetLogFilePath();

            json = System.Text.Json.JsonSerializer.Serialize<StateLoggerModel>(stateLoggerModel);
            File.WriteAllText(this.StateLogFile, json);
        }

        // This method is used to add a log to the daily log file
        public void AddToDailyLogJson(DailyLoggerModel dailyLoggerModel)
        {
            DailyLogs.Add(dailyLoggerModel);
        }

        // This method is used to log the daily saves
        public void logDailySaves()
        {
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
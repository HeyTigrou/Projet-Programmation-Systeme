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
        private string DailyLogJsonFile;
        private string DailyLogXmlFile;
        private string StateLogJsonFile;
        private string StateLogXmlFile;
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

            if (ConfigurationManager.AppSettings["LogsInJson"] == "Y")
            {
                DailyLogJsonFile = Path.Combine(DirectoryPath + $"{currentDate}-{FileName}.json");
                // If the file doesn't exist, we create it
                if (!File.Exists(DailyLogJsonFile))
                {
                    File.Create(DailyLogJsonFile).Close();
                }
            }
               

            if (ConfigurationManager.AppSettings["LogsInXMl"] == "Y")
            {
                DailyLogXmlFile = Path.Combine(DirectoryPath + $"{currentDate}-{FileName}.xml");
                // If the file doesn't exist, we create it
                if (!File.Exists(DailyLogXmlFile))
                {
                    File.Create(DailyLogXmlFile).Close();
                }
            }
        }

        // This method is used to create the log file that save the state of the save works
        public void logProcessFile(string processName)
        {
            if (ConfigurationManager.AppSettings["LogsInJson"] == "Y")
            {
                StateLogJsonFile = Path.Combine(StateLogDirectoryPath + $"{processName}.json");
                // If the file doesn't exist, we create it
                if (!File.Exists(StateLogJsonFile))
                {
                    File.Create(StateLogJsonFile).Close();
                }
            }

            if (ConfigurationManager.AppSettings["LogsInXMl"] == "Y")
            {
                StateLogXmlFile = Path.Combine(StateLogDirectoryPath + $"{processName}.xml");
                // If the file doesn't exist, we create it
                if (!File.Exists(StateLogXmlFile))
                {
                    File.Create(StateLogXmlFile).Close();
                }
            }
        }

        // This method is used to log the state of the save works
        public void logProcessState(StateLoggerModel stateLoggerModel)
        {
            string json;

            // We call the method that will create the log file if it doesn't exist
            SetLogFilePath();
            if (ConfigurationManager.AppSettings["LogsInJson"] == "Y")
            {
                json = System.Text.Json.JsonSerializer.Serialize<StateLoggerModel>(stateLoggerModel);

                // We serialize the object and we write it in the state log file
                File.WriteAllText(StateLogJsonFile, json);
            }

            if (ConfigurationManager.AppSettings["LogsInXMl"] == "Y")
            {
                using (TextWriter writer = new StreamWriter(StateLogXmlFile))
                {
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(stateLoggerModel.GetType());
                    x.Serialize(writer, stateLoggerModel);
                }
            }
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
            if (new FileInfo(DailyLogJsonFile).Length != 0)
            {
                filetext = File.ReadAllText(DailyLogJsonFile);
                fileData = System.Text.Json.JsonSerializer.Deserialize<List<DailyLoggerModel>>(filetext);
            }
            fileData.AddRange(DailyLogs);

            if (ConfigurationManager.AppSettings["LogsInJson"] == "Y")
            {
                DailyLogString = System.Text.Json.JsonSerializer.Serialize(fileData);

                File.WriteAllText(DailyLogJsonFile, DailyLogString);
            }

            if (ConfigurationManager.AppSettings["LogsInXMl"] == "Y")
            {
                using (TextWriter writer = new StreamWriter(DailyLogXmlFile))
                {
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fileData.GetType());
                    x.Serialize(writer, fileData);
                }
            }
        }
    }
}
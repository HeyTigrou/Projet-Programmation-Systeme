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
using System.Xml.Serialization;

namespace easy_save.Lib.Service
{
    /// <summary>
    /// This class is used to log the daily saves and the state of the save works
    /// </summary>
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
        private static Mutex mutex = new Mutex(false);

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

        /// <summary>
        /// This method is used to create the daily log file
        /// </summary>
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

        /// <summary>
        /// This method is used to create the log file that save the state of the save works
        /// </summary>
        /// <param name="processName"></param>
        public void LogProcessFile(string processName)
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

        /// <summary>
        /// This method is used to log the state of the save works
        /// </summary>
        /// <param name="stateLoggerModel"></param>
        public void LogProcessState(StateLoggerModel stateLoggerModel)
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


        /// <summary>
        /// Adds the logger model to the list.
        /// </summary>
        /// <param name="dailyLoggerModel"></param>
        public void AddToDailyLog(DailyLoggerModel dailyLoggerModel)
        {
            DailyLogs.Add(dailyLoggerModel);
        }

        /// <summary>
        /// This method is used to log the daily logger list, it deserializes the existing log file to a list and adds the DailyLogs list to the end.
        /// </summary>
        public void LogDailySaves()
        {
            mutex.WaitOne();
            // We call the method that will create the log file if it doesn't exist
            SetLogFilePath();
            string DailyLogString;
            string filetext;

            List<DailyLoggerModel> fileDataJson = new List<DailyLoggerModel>();
            List<DailyLoggerModel> fileDataXml = new List<DailyLoggerModel>();

            if (ConfigurationManager.AppSettings["LogsInJson"] == "Y")
            {
                // Gets the file text if its not empty and deserializes it into a list of DailyLoggerModel.
                if (new FileInfo(DailyLogJsonFile).Length != 0)
                {
                    filetext = File.ReadAllText(DailyLogJsonFile);
                    fileDataJson = System.Text.Json.JsonSerializer.Deserialize<List<DailyLoggerModel>>(filetext);
                }

                // Adds the DailyLogs list to the end, this list is the list of model containing the information on the saves of this process.
                fileDataJson.AddRange(DailyLogs);


                // Serializes the list and writes it on the file.
                DailyLogString = System.Text.Json.JsonSerializer.Serialize(fileDataJson);
                File.WriteAllText(DailyLogJsonFile, DailyLogString);
            }
            if (ConfigurationManager.AppSettings["LogsInXMl"] == "Y")
            {
                // Gets the file text if its not empty and deserializes it into a list of DailyLoggerModel.
                if (new FileInfo(DailyLogXmlFile).Length != 0)
                {
                    string xmlFileText = File.ReadAllText(DailyLogXmlFile);

                    fileDataXml = (List<DailyLoggerModel>)ConvertXmlStringtoObject<List<DailyLoggerModel>>(xmlFileText);
                }

                fileDataXml.AddRange(DailyLogs);

                // Serializes the list and writes it on the file.
                using (TextWriter writer = new StreamWriter(DailyLogXmlFile))
                {
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fileDataXml.GetType());
                    x.Serialize(writer, fileDataXml);
                }
            }
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// This method is used to deserialize the xml log file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        private static T ConvertXmlStringtoObject<T>(string xmlString)
        {
            T classObject;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xmlString))
            {
                classObject = (T)xmlSerializer.Deserialize(stringReader);
            }
            return classObject;
        }
    }
}
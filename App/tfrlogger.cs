using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Enumeration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Xml;

namespace Poc
{
    public class TFRJsonObject
    {
        public string name { set; get; }
        public string sourceFilePath { set; get; }
        public string targetFilePath { set; get; }
        public string state { set; get; }
        public int totalFileToCopy { set; get; }
        public int totalFileSize { set; get; }
        public int nbFilesLeft { set; get; }
        public int progression { set; get; }
    }

    public class TFRLogger
{
        private string fileName;
        private DateTime date;
        private string logFilePath;
        private string directoryPath;

        private void setLogFilePath()
        {
            date = DateTime.Today;
            string currentDate = date.ToString("yyyy-MM-dd");
            this.logFilePath = directoryPath + currentDate + "-" + fileName + ".log";
            if (!File.Exists(this.logFilePath))
            {
                File.Create(this.logFilePath).Close();
            }
        }

        private void checkDate()
        {
            if (date != DateTime.Today)
            {
                setLogFilePath();
            }
        }

        public TFRLogger(string fileName, string directoryPath)
        {
            this.fileName = fileName;
            this.directoryPath = directoryPath;
            this.date = DateTime.Today;
            setLogFilePath();
        }

        public void appendJsonToLogFile(string name, string sourceFilePath, string targetFilePath, string state, int totalFileToCopy, int totalFileSize, int nbFilesLeft, int progression)
        {
            string json;
            List<TFRJsonObject> fileData = new List<TFRJsonObject>();
            checkDate();
            if (new FileInfo(this.logFilePath).Length != 0)
            {
                json = File.ReadAllText(this.logFilePath);
                fileData = JsonConvert.DeserializeObject<List<TFRJsonObject>>(json);
            }
            fileData.Add(
                                new TFRJsonObject
                                {
                                    name = name,
                                    sourceFilePath = sourceFilePath,
                                    targetFilePath = targetFilePath,
                                    state = state,
                                    totalFileToCopy = totalFileToCopy,
                                    totalFileSize = totalFileSize,
                                    nbFilesLeft = nbFilesLeft,
                                    progression = progression
                                }
                       );
            json = JsonConvert.SerializeObject(fileData, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(this.logFilePath, json);
        }
    }
}

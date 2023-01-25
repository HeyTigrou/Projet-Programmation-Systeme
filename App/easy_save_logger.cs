﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Enumeration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Xml;

namespace EasySave
{
    public class EasySaveDailyLoggerObject
    {
        public string name { set; get; }
        public string sourceFilePath { set; get; }
        public string targetFilePath { set; get; }
        public int filesize { set; get; }
        public DateTime FileTransferTime { set; get; }
        public DateTime time { set; get; }
    }
    public class EasySaveRealtimeLoggerObject
    {
        public string name { set; get; }
        public string sourceFilePath { set; get; }
        public string targetFilePath { set; get; }
        public string state { set; get; }
        public int totalFileToCopy { set; get; }
        public int totalFileSize { set; get; }
        public int nbFilesLeft { set; get; }
    }

    public class EasySaveLogger
    {
        private string fileName;
        private DateTime date;
        private string realtimeLogFilePath;
        private string dailyLogFilePath;
        private string directoryPath;

        private void setLogFilePath()
        {
            date = DateTime.Today;
            string currentDate = date.ToString("yyyy-MM-dd");
            this.realtimeLogFilePath = directoryPath + currentDate + "-" + fileName + ".log";
            this.dailyLogFilePath = directoryPath + fileName + ".log";
            if (!File.Exists(this.realtimeLogFilePath))
            {
                File.Create(this.realtimeLogFilePath).Close();
            }
            if (!File.Exists(this.dailyLogFilePath))
            {
                File.Create(this.dailyLogFilePath).Close();
            }
        }

        private void checkDate()
        {
            if (date != DateTime.Today)
            {
                setLogFilePath();
            }
        }

        public EasySaveLogger(string fileName, string directoryPath)
        {
            this.fileName = fileName;
            this.directoryPath = directoryPath;
            this.date = DateTime.Today;
            setLogFilePath();
        }

        public void logSaveFolderFilesProgression(string name, string sourceFilePath, string targetFilePath, string state, int totalFileToCopy, int totalFileSize, int nbFilesLeft)
        {
            string json;
            List<EasySaveRealtimeLoggerObject> fileData = new List<EasySaveRealtimeLoggerObject>();
            checkDate();
            if (new FileInfo(this.realtimeLogFilePath).Length != 0)
            {
                json = File.ReadAllText(this.realtimeLogFilePath);
                fileData = JsonConvert.DeserializeObject<List<EasySaveRealtimeLoggerObject>>(json);
            }
            fileData.Add(
                                new EasySaveRealtimeLoggerObject
                                {
                                    name = name,
                                    sourceFilePath = sourceFilePath,
                                    targetFilePath = targetFilePath,
                                    state = state,
                                    totalFileToCopy = totalFileToCopy,
                                    totalFileSize = totalFileSize,
                                    nbFilesLeft = nbFilesLeft
                                }
                       );
            json = JsonConvert.SerializeObject(fileData, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(this.realtimeLogFilePath, json);
        }

        public void logSaveFolderFiles(string name, string sourceFilePath, string targetFilePath, int filesize, DateTime FileTransferTime, DateTime time)
        {
            string json;
            List<EasySaveDailyLoggerObject> fileData = new List<EasySaveDailyLoggerObject>();
            checkDate();
            if (new FileInfo(this.dailyLogFilePath).Length != 0)
            {
                json = File.ReadAllText(this.dailyLogFilePath);
                fileData = JsonConvert.DeserializeObject<List<EasySaveDailyLoggerObject>>(json);
            }
            fileData.Add(
                                new EasySaveDailyLoggerObject
                                {
                                    name = name,
                                    sourceFilePath = sourceFilePath,
                                    targetFilePath = targetFilePath,
                                    filesize = filesize,
                                    FileTransferTime = FileTransferTime,
                                    time = time
                                }
                       );
            json = JsonConvert.SerializeObject(fileData, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(this.dailyLogFilePath, json);
        }
    }
}
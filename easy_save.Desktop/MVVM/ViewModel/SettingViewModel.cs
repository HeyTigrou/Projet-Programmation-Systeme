using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using easy_save.Desktop.Utilities;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using System.Configuration;
using DetectSoftware;
using easy_save.Lib.Models;
using easy_save.Lib.Service;

namespace easy_save.Desktop.MVVM.ViewModel
{
    internal class SettingViewModel
    {

        public bool JsonIsSelected { get; set; } = true;
        public bool XmlIsSelected { get; set; }
        public bool BothAreSelected { get; set; }

        public ICommand ChangeLogExtension { get; }
        public ICommand AddExtensionToEncrypt { get; }
        public ICommand RemoveExtensionToEncrypt { get; }

        public ObservableCollection<string> Extensions { get; } = new ObservableCollection<string>()
        {
            ".txt",
                        ".doc",
                        ".docx",
                        ".pdf",
                        ".xls",
                        ".xlsx",
                        ".ppt",
                        ".pptx",
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".gif",
                        ".bmp",
                        ".mp3",
                        ".wav",
                        ".mp4",
                        ".avi",
                        ".wmv",
                        ".mov",
                        ".zip",
                        ".rar",
                        ".7z",
                        ".exe",
                        ".dll",
                        ".iso",
                        ".bat",
                        ".cmd"
        };
        public ObservableCollection<string> SelectedExtensions { get; } = new ObservableCollection<string>()
        {
            
        };

        public SettingViewModel()
        {
            ChangeLogExtension = new RelayCommand(x => ChangeLogExtensions());
            RemoveExtensionToEncrypt = new RelayCommand(x => RemoveExtension(x as string));
            AddExtensionToEncrypt = new RelayCommand(x => AddExtension(x as string));
            ChangeLogExtensions();
        }

        private void RemoveExtension(string extension)
        {
            Extensions.Add(extension);
            SelectedExtensions.Remove(extension);
        }
        private void AddExtension(string extension)
        {
            Extensions.Remove(extension);
            SelectedExtensions.Add(extension);
        }
        private void ChangeLogExtensions()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        
            if (JsonIsSelected)
            {
                configuration.AppSettings.Settings["LogsInXMl"].Value = "Y";
                configuration.AppSettings.Settings["LogsInJson"].Value = "N";
            }
            else if (XmlIsSelected)
            {
                configuration.AppSettings.Settings["LogsInXMl"].Value = "N";
                configuration.AppSettings.Settings["LogsInJson"].Value = "Y";
            }
            else
            {
                configuration.AppSettings.Settings["LogsInXMl"].Value = "Y";
                configuration.AppSettings.Settings["LogsInJson"].Value = "Y";
            }

            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}

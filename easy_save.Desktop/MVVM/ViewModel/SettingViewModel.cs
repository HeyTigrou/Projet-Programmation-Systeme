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

namespace easy_save.Desktop.MVVM.ViewModel
{
    internal class SettingViewModel
    {

        public bool JsonIsSelected { get; set; } = true;
        public bool XmlIsSelected { get; set; }
        public bool BothAreSelected { get; set; }

        public ICommand ChangeLogExtension { get; }
        

        public SettingViewModel()
        {

            ChangeLogExtension = new RelayCommand(x => ChangeLogExtensions());
            ChangeLogExtensions();
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

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
        public ICommand GenerateKey { get; }

        public ObservableCollection<string> Extensions { get; } = FileExtensionModel.Instance.Extensions;
        public ObservableCollection<string> SelectedExtensions { get; } = FileExtensionModel.Instance.SelectedExtensions;

        /// <summary>
        /// Binds the buttons with the methods.
        /// </summary>
        public SettingViewModel()
        {
            GenerateKey = new RelayCommand(x => Generate64BitKey());
            ChangeLogExtension = new RelayCommand(x => ChangeLogExtensions());
            RemoveExtensionToEncrypt = new RelayCommand(x => RemoveExtension(x as string));
            AddExtensionToEncrypt = new RelayCommand(x => AddExtension(x as string));
            ChangeLogExtensions();
        }

        /// <summary>
        /// Removes the extension from the observable collection.
        /// </summary>
        /// <param name="extension"></param>
        private void RemoveExtension(string extension)
        {
            try
            {
                Extensions.Add(extension);
                SelectedExtensions.Remove(extension);
                FileExtensionModel.Instance.Extensions = Extensions;
                FileExtensionModel.Instance.SelectedExtensions = SelectedExtensions;
            }
            catch { }
        }
        
        /// <summary>
        /// Adds the extension to the observable collection.
        /// </summary>
        /// <param name="extension"></param>
        private void AddExtension(string extension)
        {
            try
            {
                Extensions.Remove(extension);
                SelectedExtensions.Add(extension);
                FileExtensionModel.Instance.Extensions = Extensions;
                FileExtensionModel.Instance.SelectedExtensions = SelectedExtensions;
            }
            catch { }
        }
        
        /// <summary>
        /// Changes the log file extension depending on the choosen
        /// </summary>
        private void ChangeLogExtensions()
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (JsonIsSelected)
                {
                    configuration.AppSettings.Settings["LogsInXMl"].Value = "N";
                    configuration.AppSettings.Settings["LogsInJson"].Value = "Y";
                }
                else if (XmlIsSelected)
                {
                    configuration.AppSettings.Settings["LogsInXMl"].Value = "Y";
                    configuration.AppSettings.Settings["LogsInJson"].Value = "N";
                }
                else
                {
                    configuration.AppSettings.Settings["LogsInXMl"].Value = "Y";
                    configuration.AppSettings.Settings["LogsInJson"].Value = "Y";
                }

                configuration.Save();

                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }

        /// <summary>
        /// Generates a cryp key.
        /// </summary>
        private void Generate64BitKey()
        {
            try
            {
                CryptService cryptService = new CryptService();
                bool key = cryptService.Generate();
            }
            catch {}
        }
    }
}

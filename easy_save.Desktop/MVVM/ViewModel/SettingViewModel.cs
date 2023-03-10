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
        public ICommand GenerateKey { get; }

        public ICommand AddExtensionToEncrypt { get; }
        public ICommand RemoveExtensionToEncrypt { get; }

        public ICommand AddExtensionPriority { get; }
        public ICommand RemoveExtensionPriority { get; }

        public ObservableCollection<string> EncryptionExtensions { get; } = FileExtensionModel.ExtensionInstance.CryptingExtensions;
        public ObservableCollection<string> SelectedEncryptionExtensions { get; } = FileExtensionModel.ExtensionInstance.SelectedCryptingExtensions;

        public ObservableCollection<string> PriorityExtensions { get; } = FileExtensionModel.PriorityInstance.PriorityExtensions;
        public ObservableCollection<string> SelectedPriorityExtensions { get; } = FileExtensionModel.PriorityInstance.SelectedPriorityExtensions;

        /// <summary>
        /// Binds the buttons with the methods.
        /// </summary>
        public SettingViewModel()
        {
            GenerateKey = new RelayCommand(x => Generate64BitKey());
            ChangeLogExtension = new RelayCommand(x => ChangeLogExtensions());
            
            RemoveExtensionToEncrypt = new RelayCommand(x => RemoveEncryptionExtension(x as string));
            AddExtensionToEncrypt = new RelayCommand(x => AddEncryptionExtension(x as string));

            RemoveExtensionPriority = new RelayCommand(x => RemovePriorityExtension(x as string));
            AddExtensionPriority = new RelayCommand(x => AddPriorityExtension(x as string));
            ChangeLogExtensions();
        }

        /// <summary>
        /// Removes the extension from the observable collection.
        /// </summary>
        /// <param name="extension"></param>
        private void RemoveEncryptionExtension(string extension)
        {
            try
            {
                EncryptionExtensions.Add(extension);
                SelectedEncryptionExtensions.Remove(extension);
                FileExtensionModel.ExtensionInstance.CryptingExtensions = EncryptionExtensions;
                FileExtensionModel.ExtensionInstance.SelectedCryptingExtensions = SelectedEncryptionExtensions;
            }
            catch { }
        }
        
        /// <summary>
        /// Adds the extension to the observable collection.
        /// </summary>
        /// <param name="extension"></param>
        private void AddEncryptionExtension(string extension)
        {
            try
            {
                EncryptionExtensions.Remove(extension);
                SelectedEncryptionExtensions.Add(extension);
                FileExtensionModel.ExtensionInstance.CryptingExtensions = EncryptionExtensions;
                FileExtensionModel.ExtensionInstance.SelectedCryptingExtensions = SelectedEncryptionExtensions;
            }
            catch { }
        }

        /// <summary>
        /// Removes the extension from the observable collection.
        /// </summary>
        /// <param name="extension"></param>
        private void RemovePriorityExtension(string extension)
        {
            try
            {
                PriorityExtensions.Add(extension);
                SelectedPriorityExtensions.Remove(extension);
                FileExtensionModel.PriorityInstance.PriorityExtensions = PriorityExtensions;
                FileExtensionModel.PriorityInstance.SelectedPriorityExtensions = SelectedPriorityExtensions;
            }
            catch { }
        }

        /// <summary>
        /// Adds the extension to the observable collection.
        /// </summary>
        /// <param name="extension"></param>
        private void AddPriorityExtension(string extension)
        {
            try
            {
                PriorityExtensions.Remove(extension);
                SelectedPriorityExtensions.Add(extension);
                FileExtensionModel.ExtensionInstance.PriorityExtensions = PriorityExtensions;
                FileExtensionModel.ExtensionInstance.SelectedPriorityExtensions = SelectedPriorityExtensions;
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

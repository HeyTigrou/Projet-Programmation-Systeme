using easy_save.Desktop.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using easy_save.Lib.Models;
using easy_save.Lib.Service;
using Newtonsoft.Json;
using System.Configuration;
using DetectSoftware;

namespace easy_save.Desktop.MVVM.ViewModel
{
    internal class SavesViewModel
        {

        public ICommand RemoveCommand { get; }
        public ICommand RefreshCommand { get; }

        public ICommand LaunchCommand { get; }
        public ICommand LaunchAllCommand { get; }

        private SaveWorkManagerService saveWorkManager = new SaveWorkManagerService();
        public ObservableCollection<SaveWorkModel> Processes { get; } = new ObservableCollection<SaveWorkModel>();

        public SavesViewModel()
        {
            RefreshCommand = new RelayCommand(x => Refresh());
            RemoveCommand = new RelayCommand(x => Remove(x as SaveWorkModel));
            LaunchCommand = new RelayCommand(x => LaunchSave(x as SaveWorkModel));
            LaunchAllCommand = new RelayCommand(x => LaunchAllSaves());
            Refresh();
        }


        private void Refresh()
        {
            Processes.Clear();

            foreach(SaveWorkModel saveWork in saveWorkManager.GetSaveWorks())
            {
                Processes.Add(saveWork);
            }
        }

        private void Remove(SaveWorkModel process)
        {
            saveWorkManager.DeleteSaveWork(process.Name);

            Refresh();
        }
        private void LaunchSave(SaveWorkModel process)
        {
            int errorCount;
            List<string> extensions = FileExtensionModel.Instance.SelectedExtensions.ToList();
            FileSaveService fileSaveService = new FileSaveService();
            errorCount = fileSaveService.SaveProcess(process, extensions);
        }
        private void LaunchAllSaves()
        {
            int[] errorCount = { 0, 0 };
            List<string> extensions = FileExtensionModel.Instance.SelectedExtensions.ToList();
            foreach (SaveWorkModel process in Processes)
            {
                try
                {
                    FileSaveService fileSaveService = new FileSaveService();
                    errorCount[1] += fileSaveService.SaveProcess(process, extensions);
                }
                catch
                {
                    errorCount[0]++;
                }
            }
        }
    }
}

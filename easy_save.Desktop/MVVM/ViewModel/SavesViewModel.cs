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
using System.Configuration;
using DetectSoftware;
using easy_save.Desktop.MVVM.View;

namespace easy_save.Desktop.MVVM.ViewModel
{
    internal class SavesViewModel
        {

        public ICommand RemoveCommand { get; }
        public ICommand RefreshCommand { get; }

        public ICommand LaunchCommand { get; }
        public ICommand LaunchAllCommand { get; }

        private SaveWorkManagerService SaveWorkManager = new SaveWorkManagerService();

        /// <summary>
        /// This observable collection contains all the processes on the selected folder. It is binded to a datagrid in the view.
        /// </summary>
        public ObservableCollection<SaveWorkModel> Processes { get; } = new ObservableCollection<SaveWorkModel>();


        /// <summary>
        /// Binds the methods with button, and adds the existing save works to the observable collection.
        /// </summary>
        public SavesViewModel()
        {
            RefreshCommand = new RelayCommand(x => Refresh());
            RemoveCommand = new RelayCommand(x => Remove(x as SaveWorkModel));
            LaunchCommand = new RelayCommand(x => LaunchSave(x as SaveWorkModel));
            LaunchAllCommand = new RelayCommand(x => LaunchAllSaves());
            Refresh();
        }

        /// <summary>
        /// Refreshes the observable collection, clears it and adds the save works.
        /// </summary>
        private void Refresh()
        {

            try
            {
                Processes.Clear();

                foreach (SaveWorkModel saveWork in SaveWorkManager.GetSaveWorks())
                {
                    Processes.Add(saveWork);
                }
            }
            catch { }
        }

        /// <summary>
        /// Removes the selected save work.
        /// </summary>
        /// <param name="process"></param>
        private void Remove(SaveWorkModel process)
        {

            try
            {
                SaveWorkManager.DeleteSaveWork(process.Name);

                Refresh();
            }
            catch { }
        }

        /// <summary>
        /// Launches the selected save work.
        /// </summary>
        /// <param name="process"></param>
        private void LaunchSave(SaveWorkModel process)
        {

            try
            {
                int errorCount;
                List<string> extensions = FileExtensionModel.Instance.SelectedExtensions.ToList();

                ProcessStateService service = new ProcessStateService();
                if (service.GetProcessState(ConfigurationManager.AppSettings["WorkProcessName"]))
                {
                    PopupProcessCannotStartView failurePopup = new PopupProcessCannotStartView();
                    failurePopup.ShowDialog();
                }
                else
                {
                    FileSaveService fileSaveService = new FileSaveService();
                    errorCount = fileSaveService.SaveProcess(process, extensions);
                }
                    
            }
            catch { }
        }

        /// <summary>
        /// Launches all the save works.
        /// </summary>
        private void LaunchAllSaves()
        {

            try
            {
                int[] errorCount = { 0, 0 };
                List<string> extensions = FileExtensionModel.Instance.SelectedExtensions.ToList();
                foreach (SaveWorkModel process in Processes)
                {
                    try
                    {
                        ProcessStateService service = new ProcessStateService();
                        if (service.GetProcessState(ConfigurationManager.AppSettings["WorkProcessName"]))
                        {
                            PopupProcessCannotStartView failurePopup = new PopupProcessCannotStartView();
                            failurePopup.ShowDialog();
                        }
                        else
                        {
                            FileSaveService fileSaveService = new FileSaveService();
                            errorCount[1] += fileSaveService.SaveProcess(process, extensions);
                        }
                    }
                    catch
                    {
                        errorCount[0]++;
                    }
                }
            }
            catch { }
        }
    }
}

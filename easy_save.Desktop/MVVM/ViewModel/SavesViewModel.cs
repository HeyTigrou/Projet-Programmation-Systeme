using DetectSoftware;
using easy_save.Desktop.MVVM.View;
using easy_save.Desktop.Utilities;
using easy_save.Lib.Models;
using easy_save.Lib.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Input;

namespace easy_save.Desktop.MVVM.ViewModel
{
    internal class SavesViewModel
    {
        public ICommand RemoveCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LaunchCommand { get; }
        public ICommand RemoveThreadCommand { get; }
        public ICommand PauseThreadCommand { get; }
        public ICommand ResumeThreadCommand { get; }

        
        public SaveWorkModel Selected { get; set; }

        private SaveWorkManagerService SaveWorkManager = new SaveWorkManagerService();

        /// <summary>
        /// This observable collection contains all the processes on the selected folder. It is binded to a datagrid in the view.
        /// </summary>
        public ObservableCollection<SaveWorkModel> Processes { get; } = new ObservableCollection<SaveWorkModel>();

        public Dictionary<SaveWorkModel, FileSaveService> FileSaveServices = new Dictionary<SaveWorkModel, FileSaveService>();
        /// <summary>
        /// Binds the methods with button, and adds the existing save works to the observable collection.
        /// </summary>
        public SavesViewModel()
        {
            // Binds the Button with the methods.
            RefreshCommand = new RelayCommand(x => Refresh());

            RemoveCommand = new RelayCommand(
                x => Remove(),
                x => CheckSelected()
            );

            LaunchCommand = new RelayCommand(
                x => LaunchSave(),
                x => CheckSelected()
            );

            RemoveThreadCommand = new RelayCommand(
                x => RemoveThread(),
                x => CheckSelected()

            );

            PauseThreadCommand = new RelayCommand(
                x => PauseThread(),
                x => CheckSelected()
            );

            ResumeThreadCommand = new RelayCommand(
                x => ResumeThread(),
                x => CheckSelected()
            );

            // Adds the existing saveworks to be shown in the view.
            Refresh();
        }

        private bool CheckSelected()
        {
            return Selected != null;
        }

        private void RemoveThread()
        {
            if (Selected == null)
            {
                return;
            }
            if (FileSaveServices.Any(p => p.Key == Selected))
            {
                FileSaveServices[Selected].Quit();
                FileSaveServices.Remove(Selected);
            }
        }

        private void PauseThread()
        {
            if (Selected == null)
            {
                return;
            }
            if (FileSaveServices.Any(p => p.Key == Selected))
            {
                FileSaveServices[Selected].Pause();
            }
        }

        private void ResumeThread()
        {
            if (Selected == null)
            {
                return;
            }
            if (FileSaveServices.Any(p => p.Key == Selected))
            {
                FileSaveServices[Selected].Resume();
            }
        }

        /// <summary>
        /// Refreshes the observable collection, clears it and adds the save works.
        /// </summary>
        private void Refresh()
        {

            // Clears the observable collection.
            Processes.Clear();

            // Refreshes it.
            foreach (SaveWorkModel saveWork in SaveWorkManager.GetSaveWorks())
            {
                Processes.Add(saveWork);
            }
        }

        /// <summary>
        /// Removes the selected save work.
        /// </summary>
        /// <param name="process"></param>
        private void Remove()
        {
            if (Selected == null)
            {
                return;
            }
            // Deletes the selected save work.
            SaveWorkManager.DeleteSaveWork(Selected.Name);

            // Refreshes the observable collection.
            Refresh();
        }

        /// <summary>
        /// Launches the selected save work.
        /// </summary>
        /// <param name="process"></param>
        private void LaunchSave()
        {
            if (Selected == null)
            {
                return;
            }
            int errorCount;
            List<string> extensions = FileExtensionModel.ExtensionInstance.SelectedCryptingExtensions.ToList();

            // Returns if the 
            ProcessStateService service = new ProcessStateService();
            if (service.GetProcessState(ConfigurationManager.AppSettings["WorkProcessName"]))
            {
                // Shows failure popup if an application is running.
                PopupProcessCannotStartView failurePopup = new PopupProcessCannotStartView();
                failurePopup.ShowDialog();
            }
            else
            {
                if (!(FileSaveServices.Any(p => p.Key == Selected)))
                {
                    FileSaveService fileSaveService = new FileSaveService();
                    fileSaveService.ThreadEnded += FileSaveService_ThreadEnded;
                    fileSaveService.SaveProcess(Selected, extensions);
                    FileSaveServices.Add(Selected, fileSaveService);
                }
            }
        }

        private void FileSaveService_ThreadEnded(object sender, SaveWorkModel e)
        {
            FileSaveServices.Remove(e);
        }

    }
}
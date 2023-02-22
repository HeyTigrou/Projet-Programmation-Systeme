using DetectSoftware;
using easy_save.Desktop.MVVM.View;
using easy_save.Desktop.Utilities;
using easy_save.Lib.Models;
using easy_save.Lib.Service;
using System;
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
        public ICommand StopCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }

        
        public SaveWorkModel Selected { get; set; }

        private SaveWorkManagerService SaveWorkManager = new SaveWorkManagerService();

        /// <summary>
        /// This observable collection contains all the processes on the selected folder. It is binded to a datagrid in the view.
        /// </summary>
        public ObservableCollection<SaveWorkModel> Processes { get; } = new ObservableCollection<SaveWorkModel>();

        public Dictionary<SaveWorkModel, FileSaveService> FileSaveServices = new Dictionary<SaveWorkModel, FileSaveService>();
        private Listener listener;
        /// <summary>
        /// Binds the methods with button, and adds the existing save works to the observable collection.
        /// </summary>
        public SavesViewModel()
        {
            listener = new Listener();
            listener.StopCommandReceived += StopReceived;
            listener.PauseCommandReceived += PauseReceived;
            listener.ResumeCommandReceived += ResumeReceived;
            listener.RefreshCommandReceived += RefreshReceived;
            listener.RemoveCommandReceived += RemoveReceived;
            listener.LaunchSaveCommandReceived += LaunchSaveReceived;

            listener.StartListening(42042);
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

            StopCommand = new RelayCommand(
                x => Stop(),
                x => CheckSelected()

            );

            PauseCommand = new RelayCommand(
                x => Pause(),
                x => CheckSelected()
            );

            ResumeCommand = new RelayCommand(
                x => Resume(),
                x => CheckSelected()
            );

            // Adds the existing saveworks to be shown in the view.
            Refresh();
        }

        private bool CheckSelected()
        {
            return Selected != null;
        }

        private void StopReceived(Object sender, string name)
        {
            SaveWorkModel? saveWorkModel = null;
            foreach (var item in FileSaveServices)
            {
                if(item.Key.Name == name)
                {
                    saveWorkModel = item.Key;
                    FileSaveServices[item.Key].Quit();
                }
            }
            if(saveWorkModel != null )
                FileSaveServices.Remove(saveWorkModel);
        }
        private void PauseReceived(Object sender, string name)
        {
            foreach (var item in FileSaveServices)
            {
                if (item.Key.Name == name)
                {
                    FileSaveServices[item.Key].Pause();
                }
            }
        }
        private void ResumeReceived(Object sender, string name)
        {
            foreach (var item in FileSaveServices)
            {
                if (item.Key.Name == name)
                {
                    FileSaveServices[item.Key].Resume();
                }
            }
        }
        private void RefreshReceived(Object sender, EventArgs e)
        {
            Refresh();
        }
        private void RemoveReceived(Object sender, string name)
        {
            // Deletes the selected save work.
            SaveWorkManager.DeleteSaveWork(name);

            // Refreshes the observable collection.
            Refresh();
        }
        private void LaunchSaveReceived(Object sender, string name)
        {
            foreach (var item in FileSaveServices)
            {
                if (item.Key.Name == name)
                {
                    Selected = item.Key;
                    LaunchSave();
                }
            }
        }

        private void Stop()
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

        private void Pause()
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

        private void Resume()
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
            listener.SendRefresh();
            // Clears the observable collection.
            Processes.Clear();

            // Refreshes it.
            foreach (SaveWorkModel saveWork in SaveWorkManager.GetSaveWorks())
            {
                listener.SendSaveWork(saveWork);
                saveWork.StateChanged += StateChangedEvent;
                saveWork.ProgressionChanged += ProgressionChangedEvent;
                Processes.Add(saveWork);
            }
        }

        private void StateChangedEvent(Object sender, string state)
        {
            listener.SendState((sender as SaveWorkModel).Name, state);
        }
        private void ProgressionChangedEvent(Object sender, string progression)
        {
            listener.SendProgression((sender as SaveWorkModel).Name, progression);
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
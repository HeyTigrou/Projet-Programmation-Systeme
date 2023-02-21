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
using easy_save.Desktop.MVVM.View;
using System.Threading;
using System.Diagnostics;

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

        private SaveWorkManagerService SaveWorkManager = new SaveWorkManagerService();

        /// <summary>
        /// This observable collection contains all the processes on the selected folder. It is binded to a datagrid in the view.
        /// </summary>
        public ObservableCollection<SaveWorkModel> Processes { get; } = new ObservableCollection<SaveWorkModel>();
        public ObservableCollection<Thread> Threads { get; } = new ObservableCollection<Thread>();
        public Dictionary<string, ManualResetEvent> ManualResetEvents { get; } = new Dictionary<string, ManualResetEvent>();
        public Dictionary<string, QuitThreadModel> QuitThreadModels { get; } = new Dictionary<string, QuitThreadModel>();
        /// <summary>
        /// Binds the methods with button, and adds the existing save works to the observable collection.
        /// </summary>
        public SavesViewModel()
        {

            // Binds the Button with the methods.
            RefreshCommand = new RelayCommand(x => Refresh());
            RemoveCommand = new RelayCommand(x => Remove(x as SaveWorkModel));
            LaunchCommand = new RelayCommand(x => LaunchSave(x as SaveWorkModel));

            RemoveThreadCommand = new RelayCommand(x => RemoveThread(x as Thread));
            PauseThreadCommand = new RelayCommand(x => PauseThread(x as Thread));
            ResumeThreadCommand = new RelayCommand(x => ResumeThread(x as Thread));

            // Adds the existing saveworks to be shown in the view.
            Refresh();
        }
        private void RemoveThread(Thread thread)
        {
            QuitThreadModels[thread.Name].QuitThread = true;
            ManualResetEvents[thread.Name].Set();
            Threads.Remove(thread);

            QuitThreadModels.Remove(thread.Name);
            ManualResetEvents.Remove(thread.Name);
        }

        private void PauseThread(Thread thread)
        {
            ManualResetEvents[thread.Name].Reset();
        }

        private void ResumeThread(Thread thread)
        {
            ManualResetEvents[thread.Name].Set();
        }

        /// <summary>
        /// Refreshes the observable collection, clears it and adds the save works.
        /// </summary>
        private void Refresh()
        {

            try
            {
                // Clears the observable collection.
                Processes.Clear();

                // Refreshes it.
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
                // Deletes the selected save work.
                SaveWorkManager.DeleteSaveWork(process.Name);

                // Refreshes the observable collection.
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
                    // launches the save work in a thread.

                    ManualResetEvent resetEvent = new ManualResetEvent(false);
                    ManualResetEvents.Add(process.Name, resetEvent);

                    QuitThreadModel quitThread = new QuitThreadModel();
                    QuitThreadModels.Add(process.Name, quitThread);

                    var thread = new Thread(() => saveWork(process, extensions, resetEvent, quitThread));
                    thread.Name= process.Name;
                    thread.Start();
                    resetEvent.Set();


                    Threads.Add(thread);
                }
                    
            }
            catch { }
        }
        
        private void saveWork(SaveWorkModel process, List<string> extensions, ManualResetEvent resetEvent, QuitThreadModel quitThread)
        {

            int errorCount;
            FileSaveService fileSaveService = new FileSaveService();
            errorCount = fileSaveService.SaveProcess(process, extensions, resetEvent, quitThread);
        }
    }
}

using easy_save.Desktop.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using RemoteEasySave.Lib.Service;
using RemoteEasySave.Lib.Models;
using System.Windows;
using System;
using RemoteEasySave.Desktop.MVVM.View;

namespace RemoteEasySave.Desktop.MVVM.ViewModel
{
    public class SavesViewModel
    {
        
        public ICommand RemoveCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LaunchCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }

        public Client ClientConnexion;
        public SaveWorkModel Selected { get; set; }


        /// <summary>
        /// This observable collection contains all the processes. It is binded to a datagrid in the view.
        /// </summary>
        public ObservableCollection<SaveWorkModel> Processes { get; } = new ObservableCollection<SaveWorkModel>();
        /// <summary>
        /// Binds the methods with buttons. 
        /// Connects with the main application.
        /// </summary>
        public SavesViewModel()
        {
            ClientConnexion = new Client(Processes);
            ClientConnexion.AddSaveWork += AddToCollection;
            ClientConnexion.ClearSaveWorkCollection += ClearCollection;
            ClientConnexion.Start();
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

        }

        private bool CheckSelected()
        {
            return Selected != null;
        }

        /// <summary>
        /// Stops a running save work.
        /// </summary>
        private void Stop()
        {
            if (Selected == null)
            {
                return;
            }
                string message = $"Stop||{Selected.Name}";
            if (!ClientConnexion.closed)
                ClientConnexion.Send(message);
            else
            {
                NotConnectedToServerPopUp failurePopup = new NotConnectedToServerPopUp();
                failurePopup.ShowDialog();
            }
        }

        /// <summary>
        /// Pauses a running save work.
        /// </summary>
        private void Pause()
        {
            if (Selected == null)
            {
                return;
            }
                string message = $"Pause||{Selected.Name}";
            if(!ClientConnexion.closed)
                ClientConnexion.Send(message);
            else
            {
                NotConnectedToServerPopUp failurePopup = new NotConnectedToServerPopUp();
                failurePopup.ShowDialog();
            }
        }

        /// <summary>
        /// Resumes a running save work.
        /// </summary>
        private void Resume()
        {
            if (Selected == null)
            {
                return;
            }
                string message = $"Resume||{Selected.Name}";
            if (!ClientConnexion.closed)
                ClientConnexion.Send(message);
            else
            {
                NotConnectedToServerPopUp failurePopup = new NotConnectedToServerPopUp();
                failurePopup.ShowDialog();
            }
        }

        /// <summary>
        /// Refreshes the observable collection, clears it and adds the save works.
        /// </summary>
        private void Refresh()
        {
            string message = "Refresh|| ";
            if (!ClientConnexion.closed)
                ClientConnexion.Send(message);
            else
            {
                NotConnectedToServerPopUp failurePopup = new NotConnectedToServerPopUp();
                failurePopup.ShowDialog();
            }
        }

        /// <summary>
        /// Removes the selected save work.
        /// </summary>
        private void Remove()
        {
            if (Selected == null)
            {
                return;
            }
            string message = $"Remove||{Selected.Name}";
            if (!ClientConnexion.closed)
                ClientConnexion.Send(message);
            else
            {
                NotConnectedToServerPopUp failurePopup = new NotConnectedToServerPopUp();
                failurePopup.ShowDialog();
            }
        }

        /// <summary>
        /// Launches the selected save work.
        /// </summary>
        private void LaunchSave()
        {
            if (Selected == null)
            {
                return;
            }
            string message = $"LaunchSave||{Selected.Name}";
            if (!ClientConnexion.closed)
                ClientConnexion.Send(message);
            else
            {
                NotConnectedToServerPopUp failurePopup = new NotConnectedToServerPopUp();
                failurePopup.ShowDialog();
            }
        }
        public void AddToCollection(Object sender, SaveWorkModel saveWorkModel)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Processes.Add(saveWorkModel);
            });
        }

        public void ClearCollection(Object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Processes.Clear();
            });
        }
    }
}
﻿using easy_save.Desktop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using easy_save.Lib.Models;
using easy_save.Lib.Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace easy_save.Desktop.MVVM.ViewModel
{
    internal class CreateViewModel : INotifyPropertyChanged
    {
        private string saveName;
        public string SaveName
        {
            get => saveName;
            set
            {
                saveName = value;
                SetProperty();
            }
        }
        private string pathSource;
        public string PathSource
        {
            get => pathSource;
            set
            {
                pathSource = value;
                SetProperty();
            }
        }
        private string pathDestination;
        public string PathDestination
        {
            get => pathDestination;
            set
            {
                pathDestination = value;
                SetProperty();
            }
        }

        public bool CompleteIsChecked { get; set; } = true;
        public int SaveType { get; set; }
        public ICommand CreateCommand { get; }

        private SaveWorkManagerService saveWorkManager = new SaveWorkManagerService();

        public CreateViewModel() 
        {
            CreateCommand = new RelayCommand(x => Create());
        }   
        private void Create()
        {
            SaveType= 1;
            if(CompleteIsChecked)
            {
                SaveType = 0;
            }
            SaveWorkModel saveWorkModel = new SaveWorkModel
            {
                Name= SaveName,
                InputPath= PathSource,
                OutputPath= PathDestination,
                SaveType = SaveType
            };
            saveWorkManager.AddSaveWork(saveWorkModel);

            SaveName = "";
            PathSource = "";
            PathDestination = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

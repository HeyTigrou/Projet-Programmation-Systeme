using easy_save.Desktop.Utilities;
using easy_save.Lib.Models;
using easy_save.Lib.Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private SaveWorkManagerService saveWorkManager = new SaveWorkManagerService();

        /// <summary>
        /// Binds the button with the Create method.
        /// </summary>
        public CreateViewModel()
        {
            CreateCommand = new RelayCommand(
                x => Create(),
                x => CheckBoxsEmpty()
            );

            SaveName = "";
            PathSource = "";
            PathDestination = "";
        }

        private bool CheckBoxsEmpty()
        {
            return ((SaveName != "") && (PathSource != "") && (PathDestination != ""));
        }

        /// <summary>
        /// Creates a save work with the entered name and paths.
        /// </summary>
        private void Create()
        {
            try
            {
                SaveType = 1;
                if (CompleteIsChecked)
                {
                    SaveType = 0;
                }
                SaveWorkModel saveWorkModel = new SaveWorkModel
                {
                    Name = SaveName,
                    InputPath = PathSource,
                    OutputPath = PathDestination,
                    SaveType = SaveType
                };
                saveWorkManager.AddSaveWork(saveWorkModel);

                SaveName = "";
                PathSource = "";
                PathDestination = "";
            }
            catch { }
        }

        private void SetProperty([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using easy_save.Desktop.Utilities;
using System.Windows.Input;

namespace easy_save.Desktop.MVVM.ViewModel
{
    class NavigationViewModel : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand SavesCommand { get; set; }
        public ICommand SettingCommand { get; set; }

        private void Home(object obj)=> CurrentView = new HomeViewModel();
        private void Create(object obj) => CurrentView = new CreateViewModel();
        private void Saves(object obj) => CurrentView = new SavesViewModel();
        private void Setting(object obj) => CurrentView = new SettingViewModel();

        public NavigationViewModel()
        {
            HomeCommand = new RelayCommand(Home);
            CreateCommand = new RelayCommand(Create);
            SavesCommand = new RelayCommand(Saves);
            SettingCommand = new RelayCommand(Setting);

            CurrentView = new HomeViewModel();
        }

    }
}

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

        public ICommand Home Command { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand SavesCommand { get; set; }
        public ICommand SettingCommand { get; set; }

        HomeViewModel homeViewModel = new HomeViewModel();
        CreateViewModel createViewModel = new CreateViewModel();
        SavesViewModel savesViewModel = new SavesViewModel();
        SettingViewModel settingViewModel = new SettingViewModel();

        private void Home(object obj)=> CurrentView = homeViewModel;
        private void Create(object obj) => CurrentView = createViewModel;
        private void Saves(object obj) => CurrentView = savesViewModel;
        private void Setting(object obj) => CurrentView = settingViewModel;

        /// <summary>
        /// Binds the Buttons to the methods, which create new views.
        /// </summary>
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

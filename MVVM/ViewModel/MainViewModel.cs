using ImgChecker.Core;

namespace ImgChecker.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand ProjectsViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }

        public ProjectsViewModel ProjectsVM { get; set; }

        public SettingsViewModel SettingsVM { get; set; }

        private object _currentView;
    }
}

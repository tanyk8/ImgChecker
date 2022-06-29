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

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }

        }

        public MainViewModel()
        {
            ProjectsVM = new ProjectsViewModel();
            //LearnVM = new LearnViewModel();
            SettingsVM = new SettingsViewModel();

            CurrentView = ProjectsVM;

            ProjectsViewCommand = new RelayCommand(o =>
            {
                CurrentView = ProjectsVM;
            });

            /*
            LearnViewCommand = new RelayCommand(o =>
            {
                CurrentView = LearnVM;
            });

            */

            SettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsVM;
            });

        }

    }
}

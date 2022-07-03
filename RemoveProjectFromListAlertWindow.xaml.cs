using System.Windows;


namespace ImgChecker
{
    /// <summary>
    /// Interaction logic for RemoveProjectFromListAlertWindow.xaml
    /// </summary>
    public partial class RemoveProjectFromListAlertWindow : Window
    {
        public RemoveProjectFromListAlertWindow()
        {
            InitializeComponent();
        }

        private void removeFromListClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


    }
}
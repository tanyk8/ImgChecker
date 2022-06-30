using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImgChecker.MVVM.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        //when light mode button is clicked
        private void LightMode_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Theme = "Light"; //set theme to Light
            Properties.Settings.Default.LightModeButton = "Disable"; //disable light mode button
            Properties.Settings.Default.DarkModeButton = "Enable"; //enable dark mode button

            //save the settings
            Properties.Settings.Default.Save();
        }

        //when dark mode button is clicked
        private void DarkMode_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Theme = "Dark"; //set theme to Dark
            Properties.Settings.Default.DarkModeButton = "Disable"; //disable dark mode button
            Properties.Settings.Default.LightModeButton = "Enable"; //enable light mode button

            //save the settings
            Properties.Settings.Default.Save();
        }
    }
}

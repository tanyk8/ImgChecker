using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImgChecker
{
    /// <summary>
    /// Interaction logic for EditProjectDetailsWindow.xaml
    /// </summary>
    public partial class EditProjectDetailsWindow : Window
    {
        public string pass_name { get; set; }
        public string pass_desc { get; set; }
        public string pass_loc { get; set; }

        private bool isOk_editProjectName;
        private bool isOk_editProjectLoc;
        private bool isOk_editProjectDesc;

        private string newProjectName;
        private string newProjectDescription;
        private string newProjectLocation;

        private string retrievedName;
        private string retrievedDes;
        private string retrievedLoc;

        public EditProjectDetailsWindow(string name, string description, string location)
        {
            InitializeComponent();

            retrievedName = name;
            retrievedDes = description;
            retrievedLoc = location;

            isOk_editProjectName = true;
            isOk_editProjectLoc = true;
            isOk_editProjectDesc = true;

            edit_name_txtbox.Text = name;
            edit_desc_txtbox.Text = description;
            edit_loc_txtbox.Text = location;


            //Modified by VL
            //hide error message according to theme
            if (Properties.Settings.Default.Theme == "Light")
            {
                edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                edit_desc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
            }
            else if (Properties.Settings.Default.Theme == "Dark")
            {
                edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                edit_desc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
            }

        }

    }
}
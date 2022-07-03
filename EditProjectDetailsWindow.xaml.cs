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


        //here

        private void edit_name_textChanged(object sender, TextChangedEventArgs e)
        {
            string path_projectName = @"" + edit_loc_txtbox.Text + "\\" + edit_name_txtbox.Text;

            if (!Directory.Exists(path_projectName))
            {
                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                //NEW
                if (String.IsNullOrEmpty(edit_loc_txtbox.Text))
                {

                    edit_loc_error.Content = "This field is required and must not be left empty.";
                    edit_loc_error.Foreground = Brushes.Red;
                    isOk_editProjectLoc = false;

                }

            }

            //check if the textbox is empty or not
            if (String.IsNullOrEmpty(edit_name_txtbox.Text)) //if its empty, display error
            {
                edit_name_error.Content = "This field is required and must not be left empty."; //Modified by VL
                edit_name_error.Foreground = Brushes.Red;
                isOk_editProjectName = false;

                //NEW
                //if project location not empty
                //clear whatever error message at location
                if (!String.IsNullOrEmpty(edit_loc_txtbox.Text))
                {

                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    newProjectLocation = edit_loc_txtbox.Text; //update path
                    isOk_editProjectLoc = true;

                }

            }
            else if (Directory.Exists(path_projectName)) //if project name already exists in the selected folder
            {

                edit_name_error.Content = "This project name already exists in the selected folder."; //Modified by VL
                edit_name_error.Foreground = Brushes.Red;
                isOk_editProjectName = false;

                edit_loc_error.Foreground = Brushes.Red;
                edit_loc_error.Content = "This folder already has a project called "
                    + "\"" + edit_name_txtbox.Text + "\"."; //Modified by VL



                //if project name ending with "\" or "/" character
                //display correct error message (cannot use prohibited characters) for project name
                //hide error for location
                if (edit_name_txtbox.Text.EndsWith("\\") || edit_name_txtbox.Text.EndsWith("/"))
                {

                    edit_name_error.Content = "Project name cannot contain any of the following characters: \\ / : * ? \" < > |";


                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    newProjectLocation = edit_loc_txtbox.Text; //update path
                    isOk_editProjectLoc = true;

                }

            }
            else if (edit_name_txtbox.Text.Contains(","))
            {
                edit_name_error.Content = "Project name cannot contain commas."; //Modified by VL
                edit_name_error.Foreground = Brushes.Red;
                isOk_editProjectName = false;

            }
            else if (Regex.IsMatch(edit_name_txtbox.Text, @"[""/\\:*?<>|]"))
            {

                edit_name_error.Content = "Project name cannot contain any of the following characters: \\ / : * ? \" < > |";
                edit_name_error.Foreground = Brushes.Red;
                isOk_editProjectName = false;

            }
            else  //if not empty and is a new project name in seleted folder
            {

                newProjectName = edit_name_txtbox.Text; //update name

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_editProjectName = true;

            }

            if ((edit_name_txtbox.Text == retrievedName) && (edit_loc_txtbox.Text == retrievedLoc))
            {

                newProjectName = edit_name_txtbox.Text;
                newProjectLocation = edit_loc_txtbox.Text;

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_editProjectName = true;
                isOk_editProjectLoc = true;

            }

            //if (isOk_editProjectNameLoc)
            //{
            //    edit_name_error.Foreground = Brushes.White;
            //    edit_loc_error.Foreground = Brushes.White;
            //}
            //else
            //{
            //    edit_name_error.Foreground = Brushes.Red;
            //    edit_loc_error.Foreground = Brushes.Red;

            //    edit_name_error.Content = "This project name already exists in the selected folder";
            //    edit_loc_error.Content = "This folder already has a project called "
            //        + "\"" + edit_name_txtbox.Text + "\"";
            //}

        }

        private void edit_desc_textChanged(object sender, TextChangedEventArgs e)
        {
            //check if the textbox is empty or not
            if (String.IsNullOrEmpty(edit_desc_txtbox.Text))
            {
                newProjectDescription = "None"; //update project description

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    edit_desc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    edit_desc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_editProjectDesc = true;
            }
            else //if not empty
            {

                if (edit_desc_txtbox.Text.Contains(","))
                {
                    edit_desc_error.Content = "Description cannot contain commas."; //Modified by VL
                    edit_desc_error.Foreground = Brushes.Red;
                    isOk_editProjectDesc = false;

                }
                else
                {

                    newProjectDescription = edit_desc_txtbox.Text; //update project description

                    //Modified by VL
                    //hide error message according to theme
                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        edit_desc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        edit_desc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    isOk_editProjectDesc = true;

                }

            }
        }

        private void edit_loc_textChanged(object sender, TextChangedEventArgs e)
        {
            string folder_location = edit_loc_txtbox.Text; //get user input path
            string path_projectName = @"" + edit_loc_txtbox.Text + "\\" + edit_name_txtbox.Text;

            if (!Directory.Exists(path_projectName))
            {
                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

            }

            if (String.IsNullOrEmpty(edit_loc_txtbox.Text)) //if its empty, display error
            {

                edit_loc_error.Content = "This field is required and must not be left empty."; //Modified by VL
                edit_loc_error.Foreground = Brushes.Red;
                isOk_editProjectLoc = false;

            }
            else if (!Directory.Exists(folder_location)) //if invalid folder path (folder does not exist), display error
            {

                edit_loc_error.Content = "Invalid folder path."; //Modified by VL
                edit_loc_error.Foreground = Brushes.Red;
                isOk_editProjectLoc = false;

            }
            else if (Directory.Exists(path_projectName)) //check if project name already exists in seleted folder
            {

                edit_loc_error.Content = "This folder already has a project called "
                    + "\"" + edit_name_txtbox.Text + "\"."; //Modified by VL
                edit_loc_error.Foreground = Brushes.Red;
                isOk_editProjectLoc = false;

                edit_name_error.Foreground = Brushes.Red;
                edit_name_error.Content = "This project name already exists in the selected folder."; //Modified by VL



                //if project name is empty, location there should not display any error message
                //for now....
                //project name error == yes
                //project location error == no

                if (String.IsNullOrEmpty(edit_name_txtbox.Text))
                {

                    edit_name_error.Content = "This field is required and must not be left empty.";

                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    newProjectLocation = edit_loc_txtbox.Text; //update path
                    isOk_editProjectLoc = true;

                }



            }
            else //if not empty and valid file path
            {

                newProjectLocation = edit_loc_txtbox.Text; //update path

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_editProjectLoc = true;

            }

            if ((edit_name_txtbox.Text == retrievedName) && (edit_loc_txtbox.Text == retrievedLoc))
            {

                newProjectName = edit_name_txtbox.Text;
                newProjectLocation = edit_loc_txtbox.Text;

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    edit_name_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    edit_loc_error.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_editProjectName = true;
                isOk_editProjectLoc = true;

            }
        }

        //here


    }
}
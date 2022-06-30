using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace ImgChecker
{

    /// <summary>
    /// Interaction logic for CreateProject.xaml
    /// </summary>
    ///
    public partial class CreateProject : Window
    {

        List<Button> buttonsAdded = new List<Button>();
        List<TextBox> textboxesAdded = new List<TextBox>();
        List<StackPanel> stackpanelsAdded = new List<StackPanel>();

        List<string> valid_files_path = new List<string>();

        private string projectName;
        private string projectDescription;
        private string projectLocation;

        private int addedFailFolderCount;

        private bool isOk_ProjectName;
        private bool isOk_ProjectLocation;
        private bool isOk_ProjectDescription;
        private bool isOk_FailFolders;


        //private bool isOk_project_name_location;


        public CreateProject()
        {
            InitializeComponent();

            //initialization
            addedFailFolderCount = 0;

            isOk_ProjectName = true;
            isOk_ProjectLocation = true;
            isOk_ProjectDescription = true;
            isOk_FailFolders = true;

            //isOk_project_name_location = true;


            //Modified by VL
            //hide error message according to theme
            if (Properties.Settings.Default.Theme == "Light")
            {
                projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                projectDescriptionError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                fail_errorlb_1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
            }
            else if (Properties.Settings.Default.Theme == "Dark")
            {
                projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                projectDescriptionError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                fail_errorlb_1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
            }


            ProjectNameTextBox.Text = "New Project";

            DescriptionTextBox.Text = "None";

            string user_document_folder_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //get user's document folder path
            LocationTextBox.Text = user_document_folder_path; //set into location textbox

            fail_txtbox_1.Text = "Reject Category";

            //check if location already exists a folder called "New Project"
            if (Directory.Exists(@"" + user_document_folder_path + "\\" + ProjectNameTextBox.Text))
            {
                projectNameError.Content = "This project name already exists in the selected folder."; //Modified by VL
                projectLocationError.Content = "This folder already has a project called "
                    + "\"" + ProjectNameTextBox.Text + "\"."; //Modified by VL
                projectNameError.Foreground = Brushes.Red;
                projectLocationError.Foreground = Brushes.Red;
                isOk_ProjectName = false;
                isOk_ProjectLocation = false;

                //isOk_project_name_location = false;

            }

        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            //checking what kind of files the user is dropping
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //cr8 new list
                List<string> invalid_files_path = new List<string>();
                List<string> invalid_files_names_ex = new List<string>();

                //accepted image files
                var strings = new List<string> {
                    ".jpeg",
                    ".jpg",
                    ".jpe",
                    ".jfif",
                    ".png",
                    ".gif",
                    ".tiff",
                    ".tif",
                    ".bmp",
                    ".dib",
                    ".rle"
                };

                //an array of files/folders' paths that has been dropped by user
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string s in files)
                {

                    //if it is a folder path
                    if (System.IO.File.GetAttributes(s).HasFlag(FileAttributes.Directory))
                    {

                        //get all the files inside the directory
                        string[] all_files_paths_in_dir_tree = Directory.GetFiles(s, "*", SearchOption.AllDirectories);

                        foreach (string s1 in all_files_paths_in_dir_tree)
                        {
                            if (strings.Contains(System.IO.Path.GetExtension(s1), StringComparer.OrdinalIgnoreCase))
                            {
                                //file has correct extension
                                valid_files_path.Add(s1);
                            }
                            else
                            {
                                //file has incorrect extension
                                invalid_files_path.Add(s1);
                            }

                        }

                    }
                    else //if is it a file path
                    {

                        if (strings.Contains(System.IO.Path.GetExtension(s), StringComparer.OrdinalIgnoreCase))
                        {
                            //file has correct extension
                            valid_files_path.Add(s);
                        }
                        else
                        {
                            //file has incorrect extension
                            invalid_files_path.Add(s);
                        }

                    }

                }

                string msg = "";

                //if there are error files
                if (invalid_files_path.Count > 0)
                {

                    //get invalid file names
                    for (int i = 0; i < invalid_files_path.Count; i++)
                    {
                        invalid_files_names_ex.Add(System.IO.Path.GetFileName(invalid_files_path[i]));
                    }

                    string[] invalid_files_names_ex_array = invalid_files_names_ex.ToArray();

                    msg = "Some files couldn't be uploaded (invalid or unsupported file format)." + Environment.NewLine;

                    //construct error message
                    for (int i = 0; i < invalid_files_names_ex_array.Length; i++)
                    {

                        msg += invalid_files_names_ex_array[i] + Environment.NewLine;

                    }

                }
                else //if there are no error files
                {

                    msg = "Upload successful.";

                }

                MessageBoxResult result = MessageBox.Show(msg); //print what files failed to upload

                //IF NECESSARY
                //FileInfo fileInfo = new FileInfo(files[i]);

                //we have all the picture file paths now!!

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //cr8 new list
            List<string> invalid_files_path = new List<string>();
            List<string> invalid_files_names_ex = new List<string>();

            //accepted image files
            var strings = new List<string> {
                    ".jpeg",
                    ".jpg",
                    ".jpe",
                    ".jfif",
                    ".png",
                    ".gif",
                    ".tiff",
                    ".tif",
                    ".bmp",
                    ".dib",
                    ".rle"
                };

            var codecs = ImageCodecInfo.GetImageEncoders();
            var codecFilter = "Image Files|";
            foreach (var codec in codecs)
            {
                codecFilter += codec.FilenameExtension + ";";
            }


            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                DefaultExt = ".jpg",
                Title = "Select a picture",
                FileName = "",
                Filter = codecFilter,
            };

            bool? response = openFileDialog.ShowDialog();

            if (response == true)
            {
                //Get Selected Files
                string[] files = openFileDialog.FileNames; //store file paths

                foreach (string s in files)
                {

                    if (strings.Contains(System.IO.Path.GetExtension(s), StringComparer.OrdinalIgnoreCase))
                    {
                        //file has correct extension
                        valid_files_path.Add(s);
                    }
                    else
                    {
                        //file has incorrect extension
                        invalid_files_path.Add(s);
                    }

                }

                string msg = "";

                //if there are error files
                if (invalid_files_path.Count > 0)
                {

                    //get invalid file names
                    for (int i = 0; i < invalid_files_path.Count; i++)
                    {
                        invalid_files_names_ex.Add(System.IO.Path.GetFileName(invalid_files_path[i]));
                    }

                    string[] invalid_files_names_ex_array = invalid_files_names_ex.ToArray();

                    msg = "Some files couldn't be uploaded (invalid or unsupported file format)." + Environment.NewLine;

                    //construct error message
                    for (int i = 0; i < invalid_files_names_ex_array.Length; i++)
                    {

                        msg += invalid_files_names_ex_array[i] + Environment.NewLine;

                    }

                }
                else //if there are no error files
                {

                    msg = "Upload successful.";

                }

                MessageBoxResult result = MessageBox.Show(msg); //print what files failed to upload

            }

        }

        private void TextBox_ProjectName(object sender, TextChangedEventArgs e)
        {

            string path_projectName = @"" + LocationTextBox.Text + "\\" + ProjectNameTextBox.Text;

            if (!Directory.Exists(path_projectName))
            {
                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                if (String.IsNullOrEmpty(LocationTextBox.Text))
                {

                    projectLocationError.Content = "This field is required and must not be left empty.";
                    projectLocationError.Foreground = Brushes.Red;
                    isOk_ProjectLocation = false;

                }

            }

            //check if the textbox is empty or not
            if (String.IsNullOrEmpty(ProjectNameTextBox.Text)) //if its empty, display error
            {
                projectNameError.Content = "This field is required and must not be left empty."; //Modified by VL
                projectNameError.Foreground = Brushes.Red;
                isOk_ProjectName = false;

                //if project location not empty
                //clear whatever error message at location
                if (!String.IsNullOrEmpty(LocationTextBox.Text))
                {

                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    projectLocation = LocationTextBox.Text; //update path
                    isOk_ProjectLocation = true;


                }


            }
            else if (Directory.Exists(path_projectName)) //if project name already exists in the selected folder
            {

                projectNameError.Content = "This project name already exists in the selected folder."; //Modified by VL
                projectNameError.Foreground = Brushes.Red;
                isOk_ProjectName = false;

                projectLocationError.Foreground = Brushes.Red;
                projectLocationError.Content = "This folder already has a project called "
                    + "\"" + ProjectNameTextBox.Text + "\"."; //Modified by VL


                //if project name ending with "\" or "/" character
                //display correct error message (cannot use prohibited characters) for project name
                //hide error for location
                if (ProjectNameTextBox.Text.EndsWith("\\") || ProjectNameTextBox.Text.EndsWith("/"))
                {

                    projectNameError.Content = "Project name cannot contain any of the following characters: \\ / : * ? \" < > |";


                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    projectLocation = LocationTextBox.Text; //update path
                    isOk_ProjectLocation = true;

                }

            }
            else if (ProjectNameTextBox.Text.Contains(","))
            {
                projectNameError.Content = "Project name cannot contain commas."; //Modified by VL
                projectNameError.Foreground = Brushes.Red;
                isOk_ProjectName = false;

            }
            else if (Regex.IsMatch(ProjectNameTextBox.Text, @"[""/\\:*?<>|]"))
            {

                projectNameError.Content = "Project name cannot contain any of the following characters: \\ / : * ? \" < > |";
                projectNameError.Foreground = Brushes.Red;
                isOk_ProjectName = false;

            }
            else  //if not empty and is a new project name in seleted folder
            {
                projectName = ProjectNameTextBox.Text; //update name

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_ProjectName = true;

                //isOk_project_name_location = true;

            }

        }

        private void TextBox_ProjectDescription(object sender, TextChangedEventArgs e)
        {
            //check if the textbox is empty or not
            if (String.IsNullOrEmpty(DescriptionTextBox.Text))
            {
                projectDescription = "None"; //update project description

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    projectDescriptionError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    projectDescriptionError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_ProjectDescription = true;

            }
            else //if not empty
            {

                if (DescriptionTextBox.Text.Contains(","))
                {
                    projectDescriptionError.Content = "Description cannot contain commas."; //Modified by VL
                    projectDescriptionError.Foreground = Brushes.Red;
                    isOk_ProjectDescription = false;

                }
                else
                {

                    projectDescription = DescriptionTextBox.Text; //update project description

                    //Modified by VL
                    //hide error message according to theme
                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        projectDescriptionError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        projectDescriptionError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    isOk_ProjectDescription = true;

                }

            }
        }

        private void TextBox_ProjectLocation(object sender, TextChangedEventArgs e)
        {

            string folder_location = LocationTextBox.Text; //get user input path
            string path_projectName = @"" + LocationTextBox.Text + "\\" + ProjectNameTextBox.Text;

            if (!Directory.Exists(path_projectName))
            {
                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    projectNameError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }
            }

            if (String.IsNullOrEmpty(LocationTextBox.Text)) //if its empty, display error
            {

                projectLocationError.Content = "This field is required and must not be left empty."; //Modified by VL
                projectLocationError.Foreground = Brushes.Red;
                isOk_ProjectLocation = false;

            }
            else if (!Directory.Exists(folder_location)) //if invalid folder path (folder does not exist), display error
            {

                projectLocationError.Content = "Invalid folder path."; //Modified by VL
                projectLocationError.Foreground = Brushes.Red;
                isOk_ProjectLocation = false;

            }
            else if (Directory.Exists(path_projectName)) //check if project name already exists in seleted folder
            {

                projectLocationError.Content = "This folder already has a project called "
                    + "\"" + ProjectNameTextBox.Text + "\"."; //Modified by VL
                projectLocationError.Foreground = Brushes.Red;
                isOk_ProjectLocation = false;

                projectNameError.Foreground = Brushes.Red;
                projectNameError.Content = "This project name already exists in the selected folder."; //Modified by VL


                //if project name is empty, location there should not display any error message
                //for now....
                //project name error == yes
                //project location error == no

                if (String.IsNullOrEmpty(ProjectNameTextBox.Text))
                {

                    projectNameError.Content = "This field is required and must not be left empty.";

                    if (Properties.Settings.Default.Theme == "Light")
                    {
                        projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                    }
                    else if (Properties.Settings.Default.Theme == "Dark")
                    {
                        projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                    }

                    projectLocation = LocationTextBox.Text; //update path
                    isOk_ProjectLocation = true;

                }

            }
            else //if not empty and valid file path
            {

                projectLocation = LocationTextBox.Text; //update path

                //Modified by VL
                //hide error message according to theme
                if (Properties.Settings.Default.Theme == "Light")
                {
                    projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F5F5"));
                }
                else if (Properties.Settings.Default.Theme == "Dark")
                {
                    projectLocationError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E40"));
                }

                isOk_ProjectLocation = true;

                //isOk_project_name_location = true;

            }

        }




        //up here
    }
}

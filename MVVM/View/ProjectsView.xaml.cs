using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImgChecker.MVVM.View
{
    /// <summary>
    /// Interaction logic for ProjectsView.xaml
    /// </summary>
    public partial class ProjectsView : UserControl
    {
        List<MenuProject> projectsDetails = new List<MenuProject>();

        List<Image> projectSettingsIcon = new List<Image>();
        List<StackPanel> project_child1Sp = new List<StackPanel>();
        List<Border> projectRows = new List<Border>();
        List<StackPanel> project_parentSp = new List<StackPanel>();

        List<Border> recentProjects = new List<Border>();

        private int project_selection;
        private int proNameSrt;
        private int proDescSrt;
        private int proCreatedSrt;
        private int proModifiedSrt;

        private Project proj = new Project();
        private string saveFilePath = "";
        private string defPath = "";
        private string uploadPath = "";
        private string passPath = "";
        private string rejectPath = "";
        private List<string> imageFiles = new List<string>();
        private List<string> pImageFiles = new List<string>();
        private List<RejectFile> fImageFiles = new List<RejectFile>();
        private int passcount = 0, rejectcount = 0, numProgress = 0, totalNum = 0;

        private List<RejectFolder> rejectFolderList = new List<RejectFolder>();

        public ProjectsView()
        {
            InitializeComponent();

            project_selection = 0;

            proNameSrt = 0;
            proDescSrt = 0;
            proCreatedSrt = 0;
            proModifiedSrt = 0;

            //target txt file
            string txt_file_name = "ProjectDetails.txt";

            //path of txt file
            string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + txt_file_name;

            //check if the ProjectDetails .txt file exists
            if (File.Exists(path)) //if it exists
            {

                if (new System.IO.FileInfo(path).Length == 0) // the text file is empty
                {

                    //hide load button
                    //btn_load.Visibility = Visibility.Hidden;

                    //collapse border (table_header)
                    table_header.Visibility = Visibility.Collapsed;

                    //collapse scrollviewer
                    dynamic_projects_list.Visibility = Visibility.Collapsed;

                    //collapse recent
                    recents_lb.Visibility = Visibility.Collapsed;

                    //display no project white fragment
                    no_project_fragment.Visibility = Visibility.Visible;

                }
                else //the text file has some data in it
                {

                    //read from file, store data into list 
                    //example data: New Project,None,C:\Users\USER\Documents,2021-10-08,2021-10-08 11:18:21

                    string[] lines = File.ReadAllLines(path);

                    foreach (string line in lines)
                    {
                        string[] col = line.Split(',');

                        DateTime endDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime startDate = DateTime.ParseExact(col[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        TimeSpan duration = endDate.Subtract(startDate);

                        int days = duration.Days;
                        int hours = duration.Hours;
                        int minutes = duration.Minutes;

                        string answer = "";

                        if (days != 0) //day is not 0
                        {
                            //Modified by VL
                            answer = "" + days + " days ago";
                        }
                        else //day is 0
                        {
                            if (hours != 0) //if hours is not 0
                            {
                                //Modified by VL
                                answer = "" + hours + " hours ago";
                            }
                            else
                            {
                                if (minutes != 0)
                                {
                                    //Modified by VL
                                    answer = "" + minutes + " minutes ago";
                                }
                                else
                                {
                                    answer = "Less than a minute ago"; //Modified by VL
                                }

                            }

                        }

                        // process col[0], col[1], col[2]
                        projectsDetails.Add(new MenuProject()
                        {
                            projectName = col[0],
                            projectDescription = col[1],
                            projectLocation = col[2],
                            projectCreationDate = col[3],
                            projectModifiedTime = answer

                        });

                    }

                    //collapse no project white fragment
                    no_project_fragment.Visibility = Visibility.Collapsed;

                    //show load button
                    //btn_load.Visibility = Visibility.Visible;

                    //show border (table_header)
                    table_header.Visibility = Visibility.Visible;

                    //show scrollviewer
                    dynamic_projects_list.Visibility = Visibility.Visible;

                    //show recent
                    recents_lb.Visibility = Visibility.Visible;

                    //hide search_sp
                    search_sp.Visibility = Visibility.Collapsed;

                    //load the rows of projecrs into scrollviewer


                    /*
                     *   <Border>
                                <StackPanel>

                                    <StackPanel>
                                        <TextBlock/>
                                        <TextBlock/>
                                    </StackPanel>

                                    <TextBlock />
                                    <TextBlock />
                                    <TextBlock/>
                                    <Image>
                                    </Image>
                                </StackPanel>
                            </Border>
                     * 
                     */

                    foreach (MenuProject project in projectsDetails)
                    {

                        Border border = new Border()
                        {

                            Height = 85,
                            Width = 992,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DADBDC")),
                            BorderThickness = new Thickness(0, 0, 0, 0), //Modified by VL

                        };

                        //border on click, open a window
                        border.AddHandler(Border.MouseLeftButtonDownEvent, new RoutedEventHandler(project_clicked), true);

                        Style borderStyle = this.FindResource("ProjectBorderStyle") as Style;
                        border.Style = borderStyle;

                        scrollviewer_sp.Children.Add(border);

                        StackPanel sp_parent = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Width = 992

                        };

                        //add stackpanel into border
                        border.Child = sp_parent;

                        StackPanel sp_child1 = new StackPanel
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Width = 277,
                            Margin = new Thickness(20, 17, 0, 0),


                        };

                        sp_parent.Children.Add(sp_child1);

                        TextBlock project_name_txtblock = new TextBlock
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            TextAlignment = TextAlignment.Left,
                            Text = project.projectName,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            FontSize = 17,
                            FontWeight = FontWeights.DemiBold

                        };

                        TextBlock project_location_txtblock = new TextBlock
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Text = project.projectLocation,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            FontSize = 16,
                            Foreground = Brushes.Gray

                        };

                        sp_child1.Children.Add(project_name_txtblock);
                        sp_child1.Children.Add(project_location_txtblock);

                        TextBlock project_description_txtblock = new TextBlock
                        {
                            Width = 240,
                            Height = 60,
                            Margin = new Thickness(20, 17, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Text = project.projectDescription,
                            FontSize = 16,
                            TextWrapping = TextWrapping.WrapWithOverflow,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            Foreground = Brushes.Black

                        };

                        TextBlock project_date_created_txtblock = new TextBlock
                        {
                            Width = 194,
                            Margin = new Thickness(20, 31, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Text = project.projectCreationDate,
                            FontSize = 16,
                            Height = 60,
                            Foreground = Brushes.Black

                        };

                        TextBlock project_last_modified_txtblock = new TextBlock
                        {
                            Width = 100,
                            TextWrapping = TextWrapping.WrapWithOverflow,
                            Margin = new Thickness(20, 31, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Text = project.projectModifiedTime,
                            FontSize = 16,
                            Height = 60,
                            Foreground = Brushes.Black

                        };


                        //Modified by VL
                        //set font color according to theme
                        if (Properties.Settings.Default.Theme == "Light")
                        {
                            project_name_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
                            project_location_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
                            project_description_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
                            project_date_created_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
                            project_last_modified_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
                        }
                        else if (Properties.Settings.Default.Theme == "Dark")
                        {
                            project_name_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F1F1"));
                            project_location_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F1F1"));
                            project_description_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F1F1"));
                            project_date_created_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F1F1"));
                            project_last_modified_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F1F1"));
                        }


                        Image project_settings_img = new Image
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            Width = 27,
                            Margin = new Thickness(50, 30, 0, 0),
                            Cursor = Cursors.Hand

                        };

                        project_settings_img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(projectSettings_click);


                        //Modified by VL
                        //set image according to theme
                        if (Properties.Settings.Default.Theme == "Light")
                            project_settings_img.Source = new BitmapImage(new Uri("pack://application:,,,/ImgChecker;component/Resources/menu-vertical-dark.png"));
                        else if (Properties.Settings.Default.Theme == "Dark")
                            project_settings_img.Source = new BitmapImage(new Uri("pack://application:,,,/ImgChecker;component/Resources/menu-vertical-light.png"));


                        ContextMenu m = new ContextMenu();
                        Style cmStyle = this.FindResource("ContextMenuStyle") as Style;
                        m.Style = cmStyle;

                        MenuItem m1 = new MenuItem
                        {
                            Height = 48,
                            FontSize = 13
                        };
                        m1.Header = "Show in Explorer";
                        m.Items.Add(m1);

                        MenuItem m2 = new MenuItem
                        {
                            Height = 48,
                            FontSize = 13
                        };
                        m2.Header = "Remove from list";
                        m.Items.Add(m2);

                        MenuItem m3 = new MenuItem
                        {
                            Height = 48,
                            FontSize = 13
                        };
                        m3.Header = "Edit project details";
                        m.Items.Add(m3);

                        m1.Click += new RoutedEventHandler(m1_Click);
                        m2.Click += new RoutedEventHandler(m2_Click);
                        m3.Click += new RoutedEventHandler(m3_Click);

                        project_settings_img.ContextMenu = m; //attach context menu onto the image

                        sp_parent.Children.Add(project_description_txtblock);
                        sp_parent.Children.Add(project_date_created_txtblock);
                        sp_parent.Children.Add(project_last_modified_txtblock);
                        sp_parent.Children.Add(project_settings_img);

                        projectSettingsIcon.Add(project_settings_img);  //keep track of settings img/icon
                        project_child1Sp.Add(sp_child1);
                        projectRows.Add(border);
                        project_parentSp.Add(sp_parent);

                    }

                    //check all project folder paths (if they exists)
                    //to tackle the problem where user manually go and move the project folder in explorer
                    foreach (StackPanel s in project_child1Sp)
                    {

                        var n = (TextBlock)s.Children[0]; //name
                        var p = (TextBlock)s.Children[1]; //location

                        string pth = "" + p.Text + "\\" + n.Text;

                        if (!Directory.Exists(pth))
                        {

                            //set text color to red
                            n.Foreground = Brushes.Red;
                            p.Foreground = Brushes.Red;

                        }

                    }

                    //display recent projects
                    //can only display 4 at most
                    int project_count = projectsDetails.Count;

                    List<ProjectNameModified> pnm = new List<ProjectNameModified>();

                    string[] entry = File.ReadAllLines(path);

                    foreach (string en in entry)
                    {

                        string[] column = en.Split(',');

                        // process col[0], col[1], col[2]..
                        pnm.Add(new ProjectNameModified()
                        {
                            proName = column[0],
                            proModTime = column[4]

                        });

                    }

                    pnm.Sort((x, y) => y.proModTime.CompareTo(x.proModTime));

                    /*
                      * <Border>
                             <StackPanel>
                                 <Image>
                                 </Image>
                                 <TextBlock/>
                             </StackPanel>
                         </Border>
                      * 
                   */

                    if (project_count <= 4) //if num of projects <= 4
                    {

                        foreach (ProjectNameModified item in pnm)
                        {

                            Border bdr = new Border
                            {
                                Width = 180,
                                Height = 170,
                                CornerRadius = new CornerRadius(10),
                                Margin = new Thickness(0, 0, 60, 0)
                            };

                            //border on click, open a window
                            bdr.AddHandler(Border.MouseLeftButtonDownEvent, new RoutedEventHandler(recent_project_clicked), true);

                            Style bdrStyle = this.FindResource("RecentBorderStyle") as Style;
                            bdr.Style = bdrStyle;

                            recentProjects.Add(bdr);

                            StackPanel rsp = new StackPanel
                            {
                                Height = 170,
                                Width = 180
                            };

                            bdr.Child = rsp;

                            Image recentImg = new Image
                            {
                                Width = 60,
                                Margin = new Thickness(0, 40, 0, 0)

                            };

                            recentImg.Source = new BitmapImage(new Uri("pack://application:,,,/ImgChecker;component/Resources/pic-folder.png"));

                            rsp.Children.Add(recentImg);

                            TextBlock r_txtblock = new TextBlock
                            {

                                Text = item.proName,
                                FontWeight = FontWeights.DemiBold,
                                FontSize = 17,
                                Width = 155,
                                Margin = new Thickness(0, 35, 0, 0),
                                Height = 25,
                                TextAlignment = TextAlignment.Center,
                                TextTrimming = TextTrimming.CharacterEllipsis

                            };


                            //Modified by VL
                            //set font color according to theme
                            if (Properties.Settings.Default.Theme == "Light")
                                r_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
                            else if (Properties.Settings.Default.Theme == "Dark")
                                r_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F1F1"));


                            rsp.Children.Add(r_txtblock);

                            recent_projects_sp.Children.Add(bdr);

                        }

                    }
                    else //if num of projects > 4, select 4 only
                    {
                        for (int i = 0; i < 4; i++)
                        {

                            Border bdr = new Border
                            {
                                Width = 180,
                                Height = 170,
                                CornerRadius = new CornerRadius(10),
                                Margin = new Thickness(0, 0, 60, 0)
                            };

                            //border on click, open a window
                            bdr.AddHandler(Border.MouseLeftButtonDownEvent, new RoutedEventHandler(recent_project_clicked), true);

                            Style bdrStyle = this.FindResource("RecentBorderStyle") as Style;
                            bdr.Style = bdrStyle;

                            recentProjects.Add(bdr);

                            StackPanel rsp = new StackPanel
                            {
                                Height = 170,
                                Width = 180
                            };

                            bdr.Child = rsp;

                            Image recentImg = new Image
                            {
                                Width = 60,
                                Margin = new Thickness(0, 40, 0, 0)

                            };

                            recentImg.Source = new BitmapImage(new Uri("pack://application:,,,/ImgChecker;component/Resources/pic-folder.png"));

                            rsp.Children.Add(recentImg);

                            TextBlock r_txtblock = new TextBlock
                            {

                                Text = pnm[i].proName,
                                FontWeight = FontWeights.DemiBold,
                                FontSize = 17,
                                Width = 155,
                                Margin = new Thickness(0, 35, 0, 0),
                                Height = 25,
                                TextAlignment = TextAlignment.Center,
                                TextTrimming = TextTrimming.CharacterEllipsis

                            };

                            rsp.Children.Add(r_txtblock);

                            recent_projects_sp.Children.Add(bdr);

                            if (Properties.Settings.Default.Theme == "Light")
                                r_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
                            else if (Properties.Settings.Default.Theme == "Dark")
                                r_txtblock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF1F1F1"));

                        }

                    }

                    //check if any recent project cannot be located.. if yes.. highlight in red
                    foreach (Border b in recentProjects)
                    {

                        var sp = (StackPanel)b.Child;
                        var txtblock = (TextBlock)sp.Children[1];

                        string recent_project_name = txtblock.Text; //recent project name

                        string project_folder_path = "";

                        //find project from projectsDetails list
                        foreach (MenuProject item in projectsDetails)
                        {

                            if (recent_project_name.Equals(item.projectName))
                            {
                                project_folder_path = item.projectLocation;

                                string pth = "" + project_folder_path + "\\" + recent_project_name;

                                if (!Directory.Exists(pth))
                                {
                                    //set text color to red
                                    txtblock.Foreground = Brushes.Red;

                                }

                                break;

                            }

                        }


                    }

                }

            }
            else //if does not exists
            {

                File.Create(path); //create the text file


                //hide load button
                //btn_load.Visibility = Visibility.Hidden;

                //collapse border (table_header)
                table_header.Visibility = Visibility.Collapsed;

                //collapse scrollviewer
                dynamic_projects_list.Visibility = Visibility.Collapsed;

                //collapse recent
                recents_lb.Visibility = Visibility.Collapsed;

                //display no project white fragment
                no_project_fragment.Visibility = Visibility.Visible;


            }

        }

        public void recent_project_clicked(object sender, RoutedEventArgs e)
        {

            //check which border clicked?
            Border clicked_recentBorder = (Border)sender;
            int rborder_index = recentProjects.IndexOf(clicked_recentBorder);

            Border b = recentProjects[rborder_index];

            var sp = (StackPanel)b.Child;
            var txtblock = (TextBlock)sp.Children[1];

            string recent_project_name = txtblock.Text; //recent project name

            string project_folder_path = "";
            string project_desc = "";
            string pro_create = "";

            int i = -1;

            //find project from projectsDetails list
            foreach (MenuProject item in projectsDetails)
            {
                i++;

                if (recent_project_name.Equals(item.projectName))
                {
                    project_folder_path = item.projectLocation;
                    project_desc = item.projectDescription;
                    pro_create = item.projectCreationDate;

                    break;

                }

            }

            //check if the project could not be found?
            if (!Directory.Exists(project_folder_path + "\\" + recent_project_name)) //textblock.Foreground == Brushes.Red
            {

                //display error message
                MessageBoxResult mb = MessageBox.Show("Project folder could not be located.");

            }
            else
            {
                //to update modified time in txtfile
                string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + "ProjectDetails.txt";

                //name
                //description
                //location
                //project create date
                //project modified date

                string content = recent_project_name + ","
                + project_desc + ","
                + project_folder_path + ","
                + pro_create + ","
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //update modified time in text file
                lineChanger(content, path, i);

                //TO BE CHANGED TO REAL SEGREGATION WINDOW
                //open image segregation window
                string filepath = project_folder_path + "\\" + recent_project_name;

                MainWindow dsw = new MainWindow(filepath, recent_project_name, project_desc, project_folder_path, pro_create, i);
                dsw.Show();

                //close main menu window
                //this.Close();
                Window win = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.Name == "SegregatorMainMenuWindow");
                win.Close();

            }

        }

        public void project_clicked(object sender, RoutedEventArgs e) //user clicked on a project row to open a project
        {

            //check which border clicked?
            Border clicked_border = (Border)sender;
            int border_index = projectRows.IndexOf(clicked_border);

            var textblock1 = (TextBlock)project_child1Sp[border_index].Children[0];
            var textblock2 = (TextBlock)project_child1Sp[border_index].Children[1];

            //check if the project could not be found?
            if (!Directory.Exists(textblock2.Text + "\\" + textblock1.Text)) //textblock.Foreground == Brushes.Red
            {

                //display error message
                MessageBoxResult mb = MessageBox.Show("Project folder could not be located.");

            }
            else
            {

                //get the project folder path
                string project_folder_path = projectsDetails[border_index].projectLocation;

                //get the project name
                string project_folder_name = projectsDetails[border_index].projectName;

                string project_desc = projectsDetails[border_index].projectDescription;

                string pro_create = projectsDetails[border_index].projectCreationDate;

                //update modified time in txtfile
                string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + "ProjectDetails.txt";

                //name
                //description
                //location
                //project create date
                //project modified date

                string content = project_folder_name + ","
                + project_desc + ","
                + project_folder_path + ","
                + pro_create + ","
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //update modified time in text file
                lineChanger(content, path, border_index);

                //TO BE CHANGED TO REAL SEGREGATION WINDOW
                //open image segregation window
                string filepath = project_folder_path + "\\" + project_folder_name;

                MainWindow dsw = new MainWindow(filepath, project_folder_name, project_desc, project_folder_path, pro_create, border_index);
                dsw.Show();

                //close main menu window
                //this.Close();
                Window win = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.Name == "SegregatorMainMenuWindow");
                win.Close();

            }

        }

        public void projectSettings_click(object sender, MouseButtonEventArgs e)
        {

            //which project's setting has been clicked (get reference)
            Image clicked_img = (Image)sender;

            project_selection = projectSettingsIcon.IndexOf(clicked_img); //which image was clicked

            ContextMenu contextMenu = clicked_img.ContextMenu;
            contextMenu.PlacementTarget = clicked_img;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
            contextMenu.IsOpen = true;

        }

        private void m1_Click(object sender, RoutedEventArgs e)
        {

            string project_path = projectsDetails[project_selection].projectLocation;

            Process.Start("explorer.exe", project_path);

        }

        private void m2_Click(object sender, RoutedEventArgs e)
        {

            //shadow the main screen
            this.Opacity = 0.5;

            //show custom message box
            RemoveProjectFromListAlertWindow mB = new RemoveProjectFromListAlertWindow();
            bool? result = mB.ShowDialog();

            //if yes
            if (result ?? false) //result == true //result.Value
            {

                string name = projectsDetails[project_selection].projectName;

                foreach (Border item in recentProjects)
                {

                    var sp = (StackPanel)item.Child;
                    var txtblock = (TextBlock)sp.Children[1];

                    string recent_project_name = txtblock.Text; //recent project name

                    if (recent_project_name.Equals(name))
                    {

                        //remove from recent projects (UI) 
                        recent_projects_sp.Children.Remove(item);

                        //remove recent project from list
                        recentProjects.Remove(item);

                        break;

                    }

                }

                //remove project detail from list
                projectsDetails.RemoveAt(project_selection);

                //rewrite txt file
                //path of txt file
                string txt_file_name = "ProjectDetails.txt";
                string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + txt_file_name;

                //read text file before clearing 
                string[] lines = File.ReadAllLines(path);

                //clear textfile
                File.WriteAllText(path, String.Empty);

                string contents = "";
                int j = 0;

                for (int i = 0; i < lines.Length; i++)
                {

                    if (i == project_selection)
                    {
                        continue;
                    }
                    else
                    {
                        string[] col = lines[i].Split(',');

                        contents = projectsDetails[j].projectName + ","
                                    + projectsDetails[j].projectDescription + ","
                                    + projectsDetails[j].projectLocation + ","
                                    + projectsDetails[j].projectCreationDate + ","
                                    + col[4];

                        File.AppendAllText(path, contents + Environment.NewLine);

                        j++;

                    }

                }

                //delete border in scrollviewer
                scrollviewer_sp.Children.Remove(projectRows[project_selection]);

                //remove from border, stk panel and image list
                projectSettingsIcon.RemoveAt(project_selection);
                projectRows.RemoveAt(project_selection);
                project_child1Sp.RemoveAt(project_selection);
                project_parentSp.RemoveAt(project_selection);

            }

            this.Opacity = 1;

        }

        private void m3_Click(object sender, RoutedEventArgs e)
        {

            //shadow the main screen
            this.Opacity = 0.5;

            //get project name
            string name = projectsDetails[project_selection].projectName;

            //get project description
            string desc = projectsDetails[project_selection].projectDescription;

            //get project location
            string location = projectsDetails[project_selection].projectLocation;

            string creation_date = projectsDetails[project_selection].projectCreationDate;

            Trace.WriteLine(creation_date);

            //open window to change name, description and location
            EditProjectDetailsWindow edit_window = new EditProjectDetailsWindow(name, desc, location);
            bool? result = edit_window.ShowDialog();

            //if yes
            if (result ?? false) //result == true //result.Value
            {

                //get updated values
                string updated_name = edit_window.pass_name;
                string updated_description = edit_window.pass_desc;
                string updated_location = edit_window.pass_loc;

                //check if selected project to edit is in recents there
                foreach (Border item in recentProjects)
                {

                    var sp = (StackPanel)item.Child;
                    var txtblock = (TextBlock)sp.Children[1];

                    string recent_project_name = txtblock.Text; //recent project name

                    if (recent_project_name.Equals(name))
                    {

                        //change name
                        txtblock.Text = updated_name;

                        break;

                    }

                }

                string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + "ProjectDetails.txt";

                //read text file before clearing 
                string[] lines = File.ReadAllLines(path);
                string[] col = lines[project_selection].Split(',');

                string content = updated_name + ","
                    + updated_description + ","
                    + updated_location + ","
                    + creation_date + ","
                    + col[4];

                Trace.WriteLine(creation_date);

                Trace.WriteLine(content);

                //update text file
                lineChanger(content, path, project_selection);

                //update textblocks in main menu's list
                var txt_whole_name = (StackPanel)project_parentSp[project_selection].Children[0];

                var txt_name = (TextBlock)txt_whole_name.Children[0];
                txt_name.Text = updated_name;

                var txt_location = (TextBlock)txt_whole_name.Children[1];
                txt_location.Text = updated_location;

                var txt_desc = (TextBlock)project_parentSp[project_selection].Children[1];
                txt_desc.Text = updated_description;

                //update projectDetails list
                projectsDetails[project_selection].projectName = updated_name;
                projectsDetails[project_selection].projectDescription = updated_description;
                projectsDetails[project_selection].projectLocation = updated_location;

                //move whole folder to the new location if either name or location has been changed
                if (!location.Equals(updated_location) || !name.Equals(updated_name))
                {

                    string sourceDirName = @"" + location + "\\" + name;
                    string destDirName = @"" + updated_location + "\\" + updated_name;

                    try
                    {
                        loadFile(sourceDirName + "\\projData.dat");
                        proj.setProjName(updated_name);
                        proj.setProjDesc(updated_description);
                        proj.setProjLocation(destDirName);
                        proj.setProjSaveLocation(destDirName + "\\projData.dat");
                        saveFile(sourceDirName);
                        Directory.Move(sourceDirName, destDirName);


                    }
                    catch (IOException exp) //if project name Foo => foo
                    {

                        MessageBoxResult mbr = MessageBox.Show(exp.Message);

                    }

                }

            }

            this.Opacity = 1;

        }

        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        //here above

    }
}

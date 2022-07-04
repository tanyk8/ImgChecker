using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImgChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> imageFiles = new List<string>();
        private List<string> pImageFiles = new List<string>();
        private List<RejectFile> fImageFiles = new List<RejectFile>();

        private List<RejectFolder> rejectFolderList = new List<RejectFolder>();

        public delegate void NextUploadDelegate();
        private bool continueProcess = false;
        private bool uploaderror = false;
        private int currUpCount = 0;
        private int totalUpCount = 0;
        List<string> uploadList = new List<string>();

        public delegate void NextRenameDelegate();
        private bool continueRenameProcess = false;
        private int currReCount = 0;
        private int totalReCount = 0;
        List<string> uploadFailList= new List<string>();

        private PanAndZoom panzoom;
        private Button pfBtn;
        private Image img, selImg, revertimg;
        private Label counter;
        private Project proj=new Project();

        private int position = 0, pagenum = 0;
        private string rejectName = "",tabname="";

        private int passcount =0,rejectcount = 0, numProgress = 0, totalNum = 0;
        private int currAllPage = 1, currPassPage = 1, currRejectPage = 1;
        private string currTabName = "All", currRejectFolder = "";
        private string activemissingfile = "";
        private string activepreviewtab = "";
        private string selectedManageReject = "";

        private string saveFilePath = "";
        private string defPath = "";
        private string uploadPath = "";
        private string passPath = "";
        private string rejectPath = "";

        private string fn;
        private string fd;
        private string fp;
        private string fc;
        private int projindex;

        //===============Pass Functionality===============
        private void btnPass_Click(object sender, RoutedEventArgs e)
        {
            //TO DO LIST: 1. when duplicate file exist at location

            string[] files = Directory.GetFiles(uploadPath);

            img = (Image)this.FindName("imgi");

            if (imageFiles.Count == 0)
            {
                setButtonStatus("btnPass", false);
                setButtonStatus("btnReject", false);
                MessageBox.Show("There are no image to be checked!\nUpload new image to continue");
                return;
            }

            int index = imageFiles.FindIndex(x => x.Contains(System.IO.Path.GetFileName(img.Source.ToString())));
            string sourceFile = uploadPath + "\\" + imageFiles.ElementAt(index);
            string destinationFile = passPath + "\\" + imageFiles.ElementAt(index);

            if (File.Exists(destinationFile) || pImageFiles.Contains(imageFiles.ElementAt(index)))
            {
                MessageBoxResult mbresult;
                mbresult = MessageBox.Show("Duplicate file name found at target location! Would you like to rename the image file name?\n" + destinationFile, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (mbresult == MessageBoxResult.Yes)
                {
                    string extpart = Path.GetExtension(destinationFile);
                    string namepart = Path.GetFileNameWithoutExtension(destinationFile);
                    bool repeat = true;
                    int incNum = 1;

                    do
                    {
                        if (File.Exists(passPath + "\\" + namepart + " (" + incNum + ")" + extpart))
                        {
                            incNum++;
                        }
                        else
                        {
                            repeat = false;
                            destinationFile = passPath + "\\" + namepart + " (" + incNum + ")" + extpart;
                        }
                    } while (repeat);

                }
                else
                {
                    return;
                }
            }

            if (File.Exists(sourceFile))
            {
                pImageFiles.Add(Path.GetFileName(destinationFile));
                imageFiles.RemoveAt(index);

                for (int count = 0; count < 10; count++)
                {
                    if (count < imageFiles.Count)
                    {
                        img = (Image)this.FindName("img" + (count + 1));
                        img.Source = setImgSource(uploadPath + "\\" + imageFiles.ElementAt(count), "sub");

                        if (count == 0)
                        {
                            img = (Image)this.FindName("imgi");
                            img.Source = setImgSource(uploadPath + "\\" + imageFiles.ElementAt(count), "main");
                        }
                    }

                    if (imageFiles.Count < 10 && count >= imageFiles.Count)
                    {
                        img = (Image)this.FindName("img" + (imageFiles.Count + 1).ToString());
                        img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                    }
                }
            }
            try
            {
                System.IO.File.Move(sourceFile, destinationFile);
            }
            catch
            {
                MessageBox.Show("Error! The file is either missing or corrupted!");
                return;
            }

            passcount++;
            numProgress++;

            counter = (Label)this.FindName("passCount");
            counter.Content = "Total Pass Count: " + passcount;
            counter = (Label)this.FindName("progressCount");
            counter.Content = "Overall progress: " + numProgress + "/" + totalNum;

            if (currTabName == "Pass")
            {
                //updatePass("load");

                currPassPage = 1;
                counter = (Label)this.FindName("passPage");
                counter.Content = currPassPage;
                changePassPage(currPassPage);

                setButtonStatus("btnPassPrev", false);
                setButtonStatus("btnPassFirst", false);


                if (pImageFiles.Count > 10)
                {
                    setButtonStatus("btnPassNext", true);
                    setButtonStatus("btnPassLast", true);
                }
                else
                {
                    setButtonStatus("btnPassNext", false);
                    setButtonStatus("btnPassLast", false);
                }

            }

            if (imageFiles.Count == 0)
            {
                img = (Image)this.FindName("img1");
                img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                img = (Image)this.FindName("imgi");
                img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));

                setButtonStatus("btnPass", false);
                setButtonStatus("btnReject", false);
                setButtonStatus("btnDeleteImg", false);

                MessageBox.Show("All image uploaded have been checked!");
            }
            else
            {
                currAllPage = 1;

                if (imageFiles.Count < 11)
                {
                    setButtonStatus("btnAllNext", false);
                    setButtonStatus("btnAllLast", false);
                }
                else
                {
                    setButtonStatus("btnAllNext", true);
                    setButtonStatus("btnAllLast", true);
                }

                setButtonStatus("btnAllPrev", false);
                setButtonStatus("btnAllFirst", false);

                counter = (Label)this.FindName("allPage");
                counter.Content = currAllPage;
            }

            if (pImageFiles.Count > 10)
            {
                setButtonStatus("btnPassNext", true);
                setButtonStatus("btnPassLast", true);
            }

            if (System.IO.Path.GetFileName(imgi.Source.ToString()) == "imagemissing.png")
            {
                //disable and hide revert, pass and reject
                pfBtn = (Button)this.FindName("btnPass");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnReject");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRevert");
                pfBtn.IsEnabled = false;
                setButtonStatus("btnDeleteImg", false);
                btnPass.Visibility = System.Windows.Visibility.Collapsed;
                btnReject.Visibility = System.Windows.Visibility.Collapsed;
                btnRevert.Visibility = System.Windows.Visibility.Collapsed;
                //show button
                pfBtn = (Button)this.FindName("btnInfo");
                pfBtn.IsEnabled = true;
                btnInfo.Visibility = System.Windows.Visibility.Visible;
                activemissingfile = "uploaded\\" + imageFiles.ElementAt(0);
                checkActive();
                img1.Opacity = 1;
            }
            else
            {
                checkActive();
            }


            panzoom = (PanAndZoom)this.FindName("border");
            panzoom.Reset();

            saveFile();
        }


        private void btnReject_Click(object sender, RoutedEventArgs e)
        {

            RejectBox.Visibility = System.Windows.Visibility.Visible;
            updateRejectFolderList();
        }

        private void updateRejectFolderList()
        {
            string[] folder = Directory.GetDirectories(rejectPath).Select(System.IO.Path.GetFileName).ToArray();


            if (rejectFolderList.Count > 0)
            {
                rejectFolderList.Clear();
            }

            foreach (string foldername in folder)
            {
                string percentage = "0.00";
                if (countSelectedCategory(foldername) != 0)
                {
                    percentage = (countSelectedCategory(foldername) / (double)rejectcount * 100).ToString("F");
                }


                rejectFolderList.Add(new RejectFolder() { rejectFolderName = System.IO.Path.GetFileName(foldername), rejectNum = "(" + countSelectedCategory(foldername) + " img) " + "(" + percentage + "%)" });
            }

            this.rejectListBox.ItemsSource = rejectFolderList;
            rejectListBox.Items.Refresh();
            this.rejectOverviewListBox.ItemsSource = rejectFolderList;
            rejectOverviewListBox.Items.Refresh();
            this.summaryListBox.ItemsSource = rejectFolderList;
            summaryListBox.Items.Refresh();
            this.rejectManageBox.ItemsSource = rejectFolderList;
            rejectManageBox.Items.Refresh();

            if (rejectFolderList.Count == 0 && currTabName == "Reject" || rejectFolderList.Count == 0 && RejectBox.Visibility.ToString() == "Visible")
            {
                MessageBox.Show("No reject category found! Please add a reject category!");
            }
        }
        private void selectReject_doubleclick(object sender, RoutedEventArgs e)
        {
            if (rejectListBox.SelectedItem != null)
            {
                RejectFolder temp = (RejectFolder)rejectListBox.SelectedItem;
                string selectedDFolder = temp.getRejectFolderName();
                // string[] files = Directory.GetFiles(uploadPath);

                img = (Image)this.FindName("imgi");

                if (imageFiles.Count == 0)
                {
                    MessageBox.Show("No image found in the upload folder!");
                    pfBtn = (Button)this.FindName("btnPass");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnReject");
                    pfBtn.IsEnabled = false;
                    return;
                }

                int index = imageFiles.FindIndex(x => x.Contains(System.IO.Path.GetFileName(img.Source.ToString())));

                string sourceFile = uploadPath + "\\" + imageFiles.ElementAt(index);
                string destinationFile = rejectPath + "\\" + selectedDFolder + "\\" + imageFiles.ElementAt(index);

                if (File.Exists(destinationFile) || checkReject(selectedDFolder, imageFiles.ElementAt(index)))
                {
                    MessageBoxResult mbresult;
                    mbresult = MessageBox.Show("Duplicate file name found at target location! Would you like to rename the image file name?\n" + destinationFile, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (mbresult == MessageBoxResult.Yes)
                    {
                        string extpart = Path.GetExtension(destinationFile);
                        string namepart = Path.GetFileNameWithoutExtension(destinationFile);

                        bool repeat = true;
                        int incNum = 1;

                        do
                        {
                            if (File.Exists(rejectPath + "\\" + selectedDFolder + "\\" + namepart + " (1)" + extpart))
                            {
                                incNum++;
                            }
                            else
                            {
                                repeat = false;
                                destinationFile = rejectPath + "\\" + selectedDFolder + "\\" + namepart + " (1)" + extpart;
                            }
                        } while (repeat);

                    }
                    else
                    {
                        return;
                    }
                }

                if (File.Exists(sourceFile))
                {
                    fImageFiles.Add(new RejectFile(selectedDFolder, Path.GetFileName(destinationFile)));
                    imageFiles.RemoveAt(index);
                    for (int count = 0; count < 10; count++)
                    {
                        if (count < imageFiles.Count)
                        {
                            img = (Image)this.FindName("img" + (count + 1));
                            img.Source = setImgSource(uploadPath + "\\" + imageFiles.ElementAt(count), "sub");

                            if (count == 0)
                            {
                                img = (Image)this.FindName("imgi");
                                img.Source = setImgSource(uploadPath + "\\" + imageFiles.ElementAt(count), "main");
                            }
                        }

                        if (imageFiles.Count < 10 && count >= imageFiles.Count)
                        {
                            img = (Image)this.FindName("img" + (imageFiles.Count + 1).ToString());
                            img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                        }
                    }
                }

                try
                {
                    System.IO.File.Move(sourceFile, destinationFile);
                }
                catch
                {
                    MessageBox.Show("Error! The file is either missing or corrupted!");
                    return;
                }

                counter = (Label)this.FindName("rejectCount");
                rejectcount++;
                numProgress++;
                counter.Content = "Total Reject Count: " + rejectcount;
                counter = (Label)this.FindName("progressCount");
                counter.Content = "Overall progress: " + numProgress + "/" + totalNum;

                if (currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
                {
                    if (selectedDFolder == currRejectFolder)
                    {
                        //updateReject("load", "content");

                        currRejectPage = 1;
                        counter = (Label)this.FindName("rejectPage");
                        counter.Content = currRejectPage;
                        changeRejectPage(currRejectPage, "content", "");

                        pfBtn = (Button)this.FindName("btnRejectPrev");
                        pfBtn.IsEnabled = false;
                        pfBtn = (Button)this.FindName("btnRejectFirst");
                        pfBtn.IsEnabled = false;


                        if (countSelectedCategory(temp.getRejectFolderName()) > 10)
                        {
                            pfBtn = (Button)this.FindName("btnRejectNext");
                            pfBtn.IsEnabled = true;
                            pfBtn = (Button)this.FindName("btnRejectLast");
                            pfBtn.IsEnabled = true;
                        }
                        else
                        {
                            pfBtn = (Button)this.FindName("btnRejectNext");
                            pfBtn.IsEnabled = false;
                            pfBtn = (Button)this.FindName("btnRejectLast");
                            pfBtn.IsEnabled = false;
                        }
                    }

                }


                if (imageFiles.Count == 0)
                {
                    img = (Image)this.FindName("img1");
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                    img = (Image)this.FindName("imgi");
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));

                    MessageBox.Show("All image uploaded have been checked!");

                    pfBtn = (Button)this.FindName("btnPass");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnReject");
                    pfBtn.IsEnabled = false;
                    setButtonStatus("btnDeleteImg", false);

                }
                else
                {
                    currAllPage = 1;

                    if (imageFiles.Count < 11)
                    {
                        pfBtn = (Button)this.FindName("btnAllNext");
                        pfBtn.IsEnabled = false;
                        pfBtn = (Button)this.FindName("btnAllLast");
                        pfBtn.IsEnabled = false;
                    }
                    else
                    {
                        pfBtn = (Button)this.FindName("btnAllNext");
                        pfBtn.IsEnabled = true;
                        pfBtn = (Button)this.FindName("btnAllLast");
                        pfBtn.IsEnabled = true;
                    }

                    pfBtn = (Button)this.FindName("btnAllPrev");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnAllFirst");
                    pfBtn.IsEnabled = false;

                    counter = (Label)this.FindName("allPage");
                    counter.Content = currAllPage;
                }

                if (pImageFiles.Count > 10)
                {
                    pfBtn = (Button)this.FindName("btnPassNext");
                    pfBtn.IsEnabled = true;
                    pfBtn = (Button)this.FindName("btnPassLast");
                    pfBtn.IsEnabled = true;
                }
                updateRejectFolderList();

                if (System.IO.Path.GetFileName(imgi.Source.ToString()) == "imagemissing.png")
                {
                    //disable and hide revert, pass and reject
                    pfBtn = (Button)this.FindName("btnPass");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnReject");
                    pfBtn.IsEnabled = false;
                    pfBtn = (Button)this.FindName("btnRevert");
                    pfBtn.IsEnabled = false;
                    setButtonStatus("btnDeleteImg", false);
                    btnPass.Visibility = System.Windows.Visibility.Collapsed;
                    btnReject.Visibility = System.Windows.Visibility.Collapsed;
                    btnRevert.Visibility = System.Windows.Visibility.Collapsed;
                    //show button
                    pfBtn = (Button)this.FindName("btnInfo");
                    pfBtn.IsEnabled = true;
                    btnInfo.Visibility = System.Windows.Visibility.Visible;
                    activemissingfile = "uploaded\\" + imageFiles.ElementAt(0);
                    checkActive();
                    img1.Opacity = 1;
                }
                else
                {
                    checkActive();
                }


                panzoom = (PanAndZoom)this.FindName("border");
                panzoom.Reset();
                RejectBox.Visibility = System.Windows.Visibility.Collapsed;
                saveFile();
            }
        }


        //=================all page navigation====================
        private void btnAllPrev_Click(object sender, RoutedEventArgs e)
            
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            //TO DO LIST: 1. when duplicate file exist at location

            var codecs = ImageCodecInfo.GetImageEncoders();
            var codecFilter = "Image Files|";
            foreach (var codec in codecs)
            {
                codecFilter += codec.FilenameExtension + ";";
            }

            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = codecFilter,
                Multiselect = true,
                DefaultExt = ".jpg",
                FileName = "",
                Title = "Select a picture",
            };

            //dialog.Filter = codecFilter;
            //var dialog = new Microsoft.Win32.OpenFileDialog();
            //dialog.Title = "Select a picture";
            //dialog.FileName = ""; // Default file name
            //dialog.DefaultExt = ".jpg"; // Default file extension
            //dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
            //  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            //  "Portable Network Graphic (*.png)|*.png"; // Filter files by extension
            //dialog.Multiselect = true;

            // Show open file dialog box
            bool? result = dialog.ShowDialog();
            uploaderror = false;

            if (result == true)
            {
                pbBar.Value = 0;
                ProgressBox.Visibility = System.Windows.Visibility.Visible;
                ProgressBox_progress.Visibility = System.Windows.Visibility.Visible;
                foreach (string file in dialog.FileNames)
                {
                    uploadList.Add(file);
                    //try
                    //{
                    //    sourceFile = file;
                    //    destinationFile = uploadPath + "\\" + System.IO.Path.GetFileName(file);
                    //    System.IO.File.Copy(sourceFile, destinationFile);
                    //    imageFiles.Add(System.IO.Path.GetFileName(file));
                    //    totalNum++;
                    //}
                    //catch
                    //{
                    //    uploadFailList.Add(file);
                    //    error = true;
                    //}
                }
                currUpCount = 0;
                totalUpCount = uploadList.Count;

                sUploadButton_Click(new object(), new EventArgs());






            }

        }

        //=================all page navigation====================
        private void btnAllPrev_Click(object sender, RoutedEventArgs e)

        {
            counter = (Label)this.FindName("allPage");
            currAllPage--;
            counter.Content = currAllPage;

            if (currAllPage != 1 || imageFiles.Count > 10)
            {
                pfBtn = (Button)this.FindName("btnAllNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnAllLast");
                pfBtn.IsEnabled = true;
            }

            if (currAllPage == 1)
            {
                pfBtn = (Button)this.FindName("btnAllPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnAllFirst");
                pfBtn.IsEnabled = false;
            }

            changePage(currAllPage);
        }

        private void btnAllNext_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("allPage");
            currAllPage++;
            counter.Content = currAllPage;

            if (currAllPage != 1)
            {
                pfBtn = (Button)this.FindName("btnAllPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnAllFirst");
                pfBtn.IsEnabled = true;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnAllPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnAllFirst");
                pfBtn.IsEnabled = false;
            }

            int finalpage = (imageFiles.Count - 1) / 10 + 1;

            if (currAllPage == finalpage)
            {
                pfBtn = (Button)this.FindName("btnAllNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnAllLast");
                pfBtn.IsEnabled = false;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnAllNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnAllLast");
                pfBtn.IsEnabled = true;
            }

            changePage(currAllPage);
        }

        private void changePage(int currPage)
        {
            int num = 1;
            for (int count = currPage * 10 - 10; count < currPage * 10; count++)
            {
                img = (Image)this.FindName("img" + num);

                if (count < imageFiles.Count)
                {
                    img.Source = setImgSource(uploadPath + "\\" + imageFiles.ElementAt(count), "sub");
                }
                else
                {
                    img = (Image)this.FindName("img" + num);
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                }
                num++;
            }
            checkActive();
        }

        private void btnAllFirst_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("allPage");
            currAllPage = 1;
            counter.Content = currAllPage;


            if (currAllPage != 1 || imageFiles.Count > 10)
            {
                pfBtn = (Button)this.FindName("btnAllNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnAllLast");
                pfBtn.IsEnabled = true;
            }

            if (currAllPage == 1)
            {
                pfBtn = (Button)this.FindName("btnAllPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnAllFirst");
                pfBtn.IsEnabled = false;
            }


            changePage(currAllPage);
        }

        private void btnAllLast_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("allPage");
            currAllPage = (imageFiles.Count - 1) / 10 + 1;
            counter.Content = currAllPage;

            if (currAllPage != 1)
            {
                pfBtn = (Button)this.FindName("btnAllPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnAllFirst");
                pfBtn.IsEnabled = true;
            }

            if (imageFiles.Count < currAllPage * 10 + 1)
            {
                pfBtn = (Button)this.FindName("btnAllNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnAllLast");
                pfBtn.IsEnabled = false;
            }
            //else
            //{
            //    pfBtn = (Button)this.FindName("btnAllNext");
            //    pfBtn.IsEnabled = true;
            //    pfBtn = (Button)this.FindName("btnAllLast");
            //    pfBtn.IsEnabled = true;
            //}

            changePage(currAllPage);
        }

        //-----------------end pass page navigation-------------------

        //=================pass page navigation====================
        private void btnPassPrev_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage--;
            counter.Content = currPassPage;

            if (currPassPage != 1 || pImageFiles.Count > 10)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = true;
            }

            if (currPassPage == 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = false;
            }

            changePassPage(currPassPage);
        }

        private void btnPassNext_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage++;
            counter.Content = currPassPage;

            if (currPassPage != 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = true;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = false;
            }

            int finalpage = (pImageFiles.Count - 1) / 10 + 1;

            if (currPassPage == finalpage)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = false;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = true;
            }

            changePassPage(currPassPage);
        }

        private void changePassPage(int currPage)
        {
            int num = 1;
            for (int count = pImageFiles.Count - ((currPage - 1) * 10 + 1); count >= pImageFiles.Count - (currPage * 10); count--)
            {
                img = (Image)this.FindName("Pimg" + num);

                if (count >= 0)
                {
                    img.Source = setImgSource(passPath + "\\" + pImageFiles.ElementAt(count), "sub");
                }
                else
                {
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                }
                num++;
            }
            checkActive();
        }

        private void btnPassFirst_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage = 1;
            counter.Content = currPassPage;




            if (currPassPage != 1 || pImageFiles.Count > 10)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = true;
            }

            if (currPassPage == 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = false;
            }


            changePassPage(currPassPage);
        }

        private void btnPassLast_Click(object sender, RoutedEventArgs e)
        {
            counter = (Label)this.FindName("passPage");
            currPassPage = (pImageFiles.Count - 1) / 10 + 1;
            counter.Content = currPassPage;

            if (currPassPage != 1)
            {
                pfBtn = (Button)this.FindName("btnPassPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnPassFirst");
                pfBtn.IsEnabled = true;
            }

            if (pImageFiles.Count < currPassPage * 10 + 1)
            {
                pfBtn = (Button)this.FindName("btnPassNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnPassLast");
                pfBtn.IsEnabled = false;
            }

            changePassPage(currPassPage);
        }

        //-----------------end pass page navigation-------------------

        //=================reject page navigation====================
        private void btnRejectPrev_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage--;
            counter.Content = currRejectPage;

            if (currRejectPage != 1 || countSelectedCategory(foldernameselected) > 10)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = true;
            }

            if (currRejectPage == 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = false;
            }

            changeRejectPage(currRejectPage, "overview", "");
        }

        private void btnRejectNext_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage++;
            counter.Content = currRejectPage;

            if (currRejectPage != 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = true;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = false;
            }

            int finalpage = (countSelectedCategory(foldernameselected) - 1) / 10 + 1;

            if (currRejectPage == finalpage)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = false;
            }
            else
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = true;
            }

            changeRejectPage(currRejectPage, "overview", "");
        }

        private void changeRejectPage(int currPage, string caseOption, string specialcase)
        {
            RejectFolder temp;
            string foldernameselected = "";
            if (rejectOverviewListBox.SelectedItem != null || rejectListBox.SelectedItem != null)
            {
                if (caseOption == "overview")
                {
                    temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
                    foldernameselected = temp.getRejectFolderName();
                }
                else if (caseOption == "content")
                {
                    temp = (RejectFolder)rejectListBox.SelectedItem;
                    foldernameselected = temp.getRejectFolderName();
                }
            }

            if (caseOption == "special")
            {
                foldernameselected = specialcase;
            }

            if (caseOption == "special1")
            {
                foldernameselected = currRejectFolder;
            }

            int num = 1;

            List<RejectFile> tempList = new List<RejectFile>();

            for (int i = 0; i < fImageFiles.Count; i++)
            {
                if (fImageFiles.ElementAt(i).getRejectFolderName() == foldernameselected)
                {
                    tempList.Add(new RejectFile(foldernameselected, fImageFiles.ElementAt(i).getRejectFileName()));
                }
            }


            for (int count = countSelectedCategory(foldernameselected) - 1; count >= countSelectedCategory(foldernameselected) - (currRejectPage * 10); count--)
            {

                img = (Image)this.FindName("Fimg" + num);

                if (count >= 0)
                {
                    string fullpath = rejectPath + "\\" + tempList.ElementAt(count).getRejectFolderName() + "\\" + tempList.ElementAt(count).getRejectFileName();
                    img.Source = setImgSource(fullpath, "sub");
                    num++;

                }

                else
                {
                    img.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                    num++;
                }

            }
            checkActive();
        }

        private void btnRejectFirst_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage = 1;
            counter.Content = currRejectPage;


            if (currRejectPage != 1 || countSelectedCategory(foldernameselected) > 10)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = true;
            }

            if (currRejectPage == 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = false;
            }


            changeRejectPage(currRejectPage, "overview", "");
        }

        private void btnRejectLast_Click(object sender, RoutedEventArgs e)
        {
            RejectFolder temp = (RejectFolder)rejectOverviewListBox.SelectedItem;
            string foldernameselected = temp.getRejectFolderName();

            counter = (Label)this.FindName("rejectPage");
            currRejectPage = (countSelectedCategory(foldernameselected) - 1) / 10 + 1;
            counter.Content = currRejectPage;

            if (currRejectPage != 1)
            {
                pfBtn = (Button)this.FindName("btnRejectPrev");
                pfBtn.IsEnabled = true;
                pfBtn = (Button)this.FindName("btnRejectFirst");
                pfBtn.IsEnabled = true;
            }

            if (countSelectedCategory(foldernameselected) < currRejectPage * 10 + 1)
            {
                pfBtn = (Button)this.FindName("btnRejectNext");
                pfBtn.IsEnabled = false;
                pfBtn = (Button)this.FindName("btnRejectLast");
                pfBtn.IsEnabled = false;
            }

            changeRejectPage(currRejectPage, "overview", "");
        }

        //-----------------end pass page navigation-------------------


        public void saveFile()
        {
            proj.setProjDatetime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            proj.setPassCount(passcount);
            proj.setRejectCount(rejectcount);
            proj.setNumProgress(numProgress);
            proj.setTotalNum(totalNum);

            proj.setProjOriFile(imageFiles);
            proj.setProjPassFile(pImageFiles);
            proj.setProjRejectFile(fImageFiles);
            proj.setProjRejectFolder(rejectFolderList);

            Project.SerializeItem(proj);
        }

        public void loadFile(string savefile)
        {
            Project.DeserializeItem(proj,savefile);

            defPath =proj.getProjLocation();
            saveFilePath =defPath+"\\projData.dat";

            uploadPath =defPath+"\\uploaded";
            passPath =defPath+"\\pass";
            rejectPath=defPath+"\\reject";

            passcount =proj.getPassCount();
            rejectcount =proj.getRejectCount();
            numProgress =proj.getNumProgress();
            totalNum =proj.getTotalNum();


            if (proj.getProjOriFile()!=null)
            {
                imageFiles = new List<string>(proj.getProjOriFile());
            }
            if (proj.getProjPassFile() != null)
            {
                pImageFiles = new List<string>(proj.getProjPassFile());
            }
            if (proj.getProjRejectFile() != null)
            {
                fImageFiles = new List<RejectFile>(proj.getProjRejectFile());
            }
            if (proj.getProjRejectFolder() != null)
            {
                rejectFolderList = new List<RejectFolder>(proj.getProjRejectFolder());
            }

        }

        private void returnhome_Click(object sender, RoutedEventArgs e)
        {
            saveFile();

            Menu opnmenu = new Menu();
            opnmenu.Show();

            this.Close();
        }

        public void handleImageMissing(string selectedImage)
        {
            //disable and hide revert, pass and reject
            pfBtn = (Button)this.FindName("btnPass");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnReject");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnRevert");
            pfBtn.IsEnabled = false;
            btnPass.Visibility = System.Windows.Visibility.Collapsed;
            btnReject.Visibility = System.Windows.Visibility.Collapsed;
            btnRevert.Visibility = System.Windows.Visibility.Collapsed;
            //show button
            pfBtn = (Button)this.FindName("btnInfo");
            pfBtn.IsEnabled = true;
            btnInfo.Visibility = System.Windows.Visibility.Visible;



            if (!selectedImage.Contains("P") && !selectedImage.Contains("F"))
            {
                position = Int32.Parse(selectedImage.Substring(3));
            }
            else if (selectedImage.Contains("P"))
            {
                position = Int32.Parse(selectedImage.Substring(4));
            }
            else if (selectedImage.Contains("F"))
            {
                position = Int32.Parse(selectedImage.Substring(4));
            }
            tabname = currTabName;
            if (currTabName == "All")
            {
                pagenum = currAllPage;
                activemissingfile = "uploaded\\" + imageFiles.ElementAt(pagenum * 10 - 10 + position - 1);
            }
            else if (currTabName == "Pass")
            {
                pagenum = currPassPage;
                activemissingfile = "pass\\" + pImageFiles.ElementAt(pImageFiles.Count - position + (pagenum * 10 - 10));
            }
            else if (currTabName == "Reject")
            {

                pagenum = currRejectPage;

                if (currRejectFolder != null || currRejectFolder.Length == 0)
                {
                    rejectName = currRejectFolder;
                }
                activemissingfile = "reject\\" + currRejectFolder + "\\" + findRejectFileName(pagenum, position, rejectName);
            }
            selImg = (Image)this.FindName("imgi");
            selImg.Source = setImgSource("pack://application:,,,/Resources/imagemissing.png", "main");
            selImg = (Image)this.FindName(selectedImage);
            selImg.Source = setImgSource("pack://application:,,,/Resources/imagemissing.png", "sub");
            checkActive();
        }

        private void btnRevert_Click(object sender, RoutedEventArgs e)
        {
            revertimg = (Image)this.FindName("imgi");
            string tempfilename = System.IO.Path.GetFileName(revertimg.Source.ToString());
            string targetfile = "", ffolder = "";
            bool pfound = false, ffound = false;
            int pindex = -1, findex = -1;

            //check the source is from pass or fail
            for (int i = 0; i < pImageFiles.Count; i++)
            {
                if (pImageFiles.ElementAt(i) == tempfilename && activepreviewtab == "Pass")
                {
                    targetfile = tempfilename;
                    pfound = true;
                    pindex = i;
                    break;
                }
            }

            if (pfound == false)
            {
                for (int i = 0; i < fImageFiles.Count; i++)
                {
                    if (fImageFiles.ElementAt(i).getRejectFileName() == tempfilename && activepreviewtab == "Reject")
                    {
                        targetfile = tempfilename;
                        ffolder = fImageFiles.ElementAt(i).getRejectFolderName();
                        ffound = true;
                        findex = i;
                        break;
                    }
                }
            }
            string targetfolder = "";
            //remove from list and add back to new list
            //move the image


            if (pfound)
            {
                string pextpart = "";
                string pnamepart = "";
                bool dupdetect = false;
                if (File.Exists(uploadPath + "\\" + tempfilename) || imageFiles.Contains(pImageFiles.ElementAt(pindex)))
                {
                    MessageBoxResult mbresult;
                    mbresult = MessageBox.Show("Duplicate file name found at target location! Would you like to rename the image file name?\n" + uploadPath + "\\" + tempfilename, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (mbresult == MessageBoxResult.Yes)
                    {
                        pextpart = Path.GetExtension(uploadPath + "\\" + tempfilename);
                        pnamepart = Path.GetFileNameWithoutExtension(uploadPath + "\\" + tempfilename);
                        dupdetect = true;
                    }
                    else
                    {
                        return;
                    }
                }

                string newrename = "";

                if (dupdetect == true)
                {

                    bool renameprocess = true;
                    int incNum = 1;
                    do
                    {
                        if (File.Exists(uploadPath + "\\" + pnamepart + " (" + incNum + ")" + pextpart))
                        {
                            incNum++;
                        }
                        else
                        {
                            System.IO.File.Move(passPath + "\\" + tempfilename, uploadPath + "\\" + pnamepart + " (" + incNum + ")" + pextpart);
                            renameprocess = false;
                            newrename = pnamepart + " (" + incNum + ")" + pextpart;
                        }

                    } while (renameprocess);
                }
                else
                {
                    System.IO.File.Move(passPath + "\\" + tempfilename, uploadPath + "\\" + tempfilename);
                    newrename = tempfilename;
                }
                imageFiles.Insert(0, newrename);
                pImageFiles.RemoveAt(pindex);

                passcount--;
                passCount.Content = "Total Pass Count: " + passcount;

            }
            else if (ffound)
            {

                string fextpart = "";
                string fnamepart = "";
                bool fdupdetect = false;

                if (File.Exists(uploadPath + "\\" + tempfilename) || imageFiles.Contains(fImageFiles.ElementAt(findex).getRejectFileName()))
                {
                    MessageBoxResult mbresult;
                    mbresult = MessageBox.Show("Duplicate file name found at target location! Would you like to rename the image file name?\n" + uploadPath + "\\" + tempfilename, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (mbresult == MessageBoxResult.Yes)
                    {
                        fextpart = Path.GetExtension(uploadPath + "\\" + tempfilename);
                        fnamepart = Path.GetFileNameWithoutExtension(uploadPath + "\\" + tempfilename);
                        fdupdetect = true;

                    }
                    else
                    {
                        return;
                    }
                }


                string newrename = "";
                if (fdupdetect)
                {
                    bool renameprocess = true;
                    int incNum = 1;
                    do
                    {
                        if (File.Exists(uploadPath + "\\" + fnamepart + " (" + incNum + ")" + fextpart))
                        {
                            incNum++;
                        }
                        else
                        {
                            System.IO.File.Move(rejectPath + "\\" + ffolder + "\\" + tempfilename, uploadPath + "\\" + fnamepart + " (" + incNum + ")" + fextpart);
                            renameprocess = false;
                            newrename = fnamepart + " (" + incNum + ")" + fextpart;
                        }

                    } while (renameprocess);

                }
                else
                {
                    try
                    {
                        System.IO.File.Move(rejectPath + "\\" + ffolder + "\\" + tempfilename, uploadPath + "\\" + tempfilename);
                        newrename = tempfilename;
                    }
                    catch
                    {
                        MessageBox.Show("The target file is missing!");
                        imgi.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
                        if (currTabName == "All")
                        {
                            changePage(currAllPage);
                        }
                        else if (currTabName == "Pass")
                        {
                            changePassPage(currPassPage);
                        }
                        else if (currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
                        {
                            changeRejectPage(currRejectPage, "special1", "");
                        }
                        return;
                    }

                }

                imageFiles.Insert(0, newrename);
                targetfolder = fImageFiles.ElementAt(findex).getRejectFolderName();
                fImageFiles.RemoveAt(findex);

                rejectcount--;
                rejectCount.Content = "Total Reject Count: " + rejectcount;
            }
            numProgress--;
            progressCount.Content = "Overall progress: " + numProgress + "/" + totalNum;

            //update counter
            updateSummaryCount();
            updateRejectFolderList();

            //update upload list
            //loadImage();

            //update view
            changePage(1);

            if (pfound && currTabName == "Pass")
            {
                changePassPage(1);
            }
            if (ffound && currTabName == "Reject" && rejectFolderContent.Visibility.ToString() == "Visible")
            {
                if (currRejectFolder != "")
                {
                    changeRejectPage(1, "special", targetfolder);
                }
            }

            //after reset set to the first image of upload
            revertimg.Source = new BitmapImage(new Uri("/Resources/noimg.png", UriKind.Relative));
            pfBtn = (Button)this.FindName("btnPass");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnReject");
            pfBtn.IsEnabled = false;
            pfBtn = (Button)this.FindName("btnRevert");
            pfBtn.IsEnabled = false;
            setButtonStatus("btnDeleteImg", false);
            btnPass.Visibility = System.Windows.Visibility.Visible;
            btnReject.Visibility = System.Windows.Visibility.Visible;
            btnRevert.Visibility = System.Windows.Visibility.Collapsed;

            checkActive();
            saveFile();
        }
    }
}
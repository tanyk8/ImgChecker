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
        List<string> uploadFailList = new List<string>();

        private PanAndZoom panzoom;
        private Button pfBtn;
        private Image img, selImg, revertimg;
        private Label counter;
        private Project proj = new Project();

        private int position = 0, pagenum = 0;
        private string rejectName = "", tabname = "";

        private int passcount = 0, rejectcount = 0, numProgress = 0, totalNum = 0;
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
            Project.DeserializeItem(proj, savefile);

            defPath = proj.getProjLocation();
            saveFilePath = defPath + "\\projData.dat";

            uploadPath = defPath + "\\uploaded";
            passPath = defPath + "\\pass";
            rejectPath = defPath + "\\reject";

            passcount = proj.getPassCount();
            rejectcount = proj.getRejectCount();
            numProgress = proj.getNumProgress();
            totalNum = proj.getTotalNum();


            if (proj.getProjOriFile() != null)
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

        public void dsw_closed(object sender, EventArgs e) //if user closes the window
        {

            //update modified time in txtfile
            //get text file path
            string path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + "ProjectDetails.txt";

            //updated content
            string content = fn + ","
                + fd + ","
                + fp + ","
                + fc + ","
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //update modified time in text file
            //change old line with new updated line
            lineChanger(content, path, projindex);

        }

        //ADD THIS
        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            //function to replace old line with new line
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        public bool checkReject(string foldername, string filename)
        {
            bool dup = false;
            for (int i = 0; i < fImageFiles.Count; i++)
            {
                if (fImageFiles.ElementAt(i).getRejectFolderName() == foldername && fImageFiles.ElementAt(i).getRejectFileName() == filename)
                {
                    dup = true;
                }
            }
            return dup;
        }
    }
}
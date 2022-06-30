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

    }
}
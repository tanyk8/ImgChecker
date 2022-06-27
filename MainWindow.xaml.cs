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



    }
}
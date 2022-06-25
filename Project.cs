using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ImgChecker
{
    [Serializable]
    class Project
    {
        private string projName;
        private string projDesc;
        private string projDatetime;
        private string projLocation;
        private string projSaveLocation;

        private int passcount;
        private int rejectcount;
        private int numProgress;
        private int totalNum;

        private List<string> projOriFile;
        private List<string> projPassFile;
        private List<RejectFile> projRejectFile;
        private List<RejectFolder> projRejectFolder;
        //counter?
        //file path for each folder?

        public Project()
        {

        }

        public Project(string projName, string projDesc, string projDatetime,string projLocation,string projSaveLocation,int passcount,int rejectcount,int numProgress,int totalNum, List<string> projOriFile, List<string> projPassFile, List<RejectFile> projRejectFile, List<RejectFolder> projRejectFolder)
        {
            this.projName = projName;
            this.projDesc = projDesc;
            this.projDatetime = projDatetime;
            this.projLocation = projLocation;
            this.projSaveLocation = projSaveLocation;

            this.passcount = passcount;
            this.rejectcount = rejectcount;
            this.numProgress = numProgress;
            this.totalNum = totalNum;

            this.projOriFile = projOriFile;
            this.projPassFile = projPassFile;
            this.projRejectFile = projRejectFile;
            this.projRejectFolder = projRejectFolder;
        }

        public string getProjName()
        {
            return projName;
        }

        public string getProjDesc()
        {
            return projDesc;
        }

        public string getProjDatetime()
        {
            return projDatetime;
        }

        public string getProjLocation()
        {
            return projLocation;
        }
        
        public string getProjSaveLocation()
        {
            return projSaveLocation;
        }

        public int getPassCount()
        {
            return passcount;
        }

        public int getRejectCount()
        {
            return rejectcount;
        }

        public int getNumProgress()
        {
            return numProgress;
        }

        public int getTotalNum()
        {
            return totalNum;
        }

        public List<string> getProjOriFile()
        {
            return projOriFile;
        }

        public List<string> getProjPassFile()
        {
            return projPassFile;
        }

        public List<RejectFile> getProjRejectFile()
        {
            return projRejectFile;
        }

        public List<RejectFolder> getProjRejectFolder()
        {
            return projRejectFolder;
        }

        public void setProjName(string projName)
        {
            this.projName = projName;
        }
        public void setProjDesc(string projDesc)
        {
            this.projDesc = projDesc;
        }
        public void setProjDatetime(string projDatetime)
        {
            this.projDatetime = projDatetime;
        }
        public void setProjLocation(string projLocation)
        {
            this.projLocation = projLocation;
        }

        public void setProjSaveLocation(string projSaveLocation)
        {
            this.projSaveLocation = projSaveLocation;
        }

        public void setPassCount(int passcount)
        {
            this.passcount = passcount;
        }

        public void setRejectCount(int rejectcount)
        {
            this.rejectcount = rejectcount;
        }

        public void setNumProgress(int numProgress)
        {
            this.numProgress = numProgress;
        }

        public void setTotalNum(int totalNum)
        {
            this.totalNum = totalNum;
        }
        public void setProjOriFile(List<string> projOriFile)
        {
            this.projOriFile = projOriFile;
        }
        public void setProjPassFile(List<string> projPassFile)
        {
            this.projPassFile = projPassFile;
        }
        public void setProjRejectFile(List<RejectFile> projRejectFile)
        {
            this.projRejectFile = projRejectFile;
        }
        public void setProjRejectFolder(List<RejectFolder> projRejectFolder)
        {
            this.projRejectFolder = projRejectFolder;
        }
        


        public static void SerializeItem(Project proj)
        {
            bool append = false;
            using (FileStream fileStream = File.Open(proj.getProjSaveLocation(), append ? FileMode.Append : FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, proj);
            }
            
        }

        public static void SerializeItemEdit(Project proj,string locationSave)
        {
            bool append = false;
            using (FileStream fileStream = File.Open(locationSave+"\\projData.dat", append ? FileMode.Append : FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, proj);
            }

        }

        public static void DeserializeItem(Project proj,string savefile)
        {
            Project project;
            using (FileStream fileStream = File.OpenRead(savefile))
            {
                var formatter = new BinaryFormatter();
                project = (Project)formatter.Deserialize(fileStream);
            }

            proj.setProjName(project.getProjName());
            proj.setProjDesc(project.getProjDesc());
            proj.setProjDatetime(project.getProjDatetime());
            proj.setProjLocation(project.getProjLocation());
            proj.setProjSaveLocation(project.getProjSaveLocation());

            proj.setPassCount(project.getPassCount());
            proj.setRejectCount(project.getRejectCount());
            proj.setNumProgress(project.getNumProgress());
            proj.setTotalNum(project.getTotalNum());

            proj.setProjOriFile(project.getProjOriFile());
            proj.setProjPassFile(project.getProjPassFile());
            proj.setProjRejectFile(project.getProjRejectFile());
            proj.setProjRejectFolder(project.getProjRejectFolder());

            
        }
    }
}

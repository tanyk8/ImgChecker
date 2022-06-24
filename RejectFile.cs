using System;

namespace ImgSegregation
{   
    [Serializable]
    class RejectFile
    {
        private string rejectFolderName;
        private string rejectFileName;

        public RejectFile(string rejectFolderName, string rejectFileName)
        {
            this.rejectFolderName = rejectFolderName;
            this.rejectFileName = rejectFileName;
        }

        public string getRejectFileName()
        {
            return rejectFileName;
        }
        public string getRejectFolderName()
        {
            return rejectFolderName;
        }

        public void setRejectFolderName(string rejectFolderName)
        {
            this.rejectFolderName = rejectFolderName;
        }

        public void setRejectFileName(string rejectFileName)
        {
            this.rejectFileName = rejectFileName;
        }
    }
}

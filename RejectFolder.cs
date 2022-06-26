using System;

namespace ImgChecker
{
    [Serializable]
    class RejectFolder
    {
        public string rejectFolderName
        {
            get;
            set;
        }
        public string rejectNum
        {
            get;
            set;
        }

        public string getRejectFolderName()
        {
            return rejectFolderName;
        }

    }
}

using System.Collections.Generic;

namespace ViewRSOM
{
    public class acqFileItem
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string folderPath { get; set; }
        public bool isChecked { get; set; }

        public List<reconFileItem> myReconFolders_list { get; set; }
        public int myReconFolders_listIndex;

        public acqFileItem(int _id, string _fileName, string _folderPath, bool _isChecked, List<reconFileItem> _myReconFolders_list, int _myReconFolders_listIndex)
        {
            id = _id;
            fileName = _fileName;
            folderPath = _folderPath;
            isChecked = _isChecked;
            myReconFolders_list = _myReconFolders_list;
            myReconFolders_listIndex = _myReconFolders_listIndex;
        }
    }
}

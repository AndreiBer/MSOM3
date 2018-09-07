using System.Collections.Generic;

namespace ViewRSOM
{
    public class studyDateItem
    {
        public int id { get; set; }
        public string folderPath { get; set; }
        public bool isChecked { get; set; }

        public List<acqFileItem> myAcqFiles_list { get; set; }
        public int myAcqFiles_listIndex;

        public studyDateItem(int _id, string _folderPath, bool _isChecked, List<acqFileItem> _myAcqFiles_list, int _myAcqFiles_listIndex)
        {
            id = _id;
            folderPath = _folderPath;
            isChecked = _isChecked;
            myAcqFiles_list = _myAcqFiles_list;
            myAcqFiles_listIndex = _myAcqFiles_listIndex;

        }
    }
}

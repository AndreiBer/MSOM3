using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewRSOM.Unmixing
{
    class UnmixFolderItem
    {
        public int id { get; set; }
        public string folderName { get; set; }
        public bool isChecked { get; set; }
        public string pathToFOlder { get; set; }


        public UnmixFolderItem(int _id, string _pathToFOlder, string _folderName, bool _isChecked)
        {
            id = _id;
            folderName = _folderName;
            isChecked = _isChecked;
            pathToFOlder = _pathToFOlder;
        }
    }
}

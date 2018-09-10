﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewRSOM.Unmixing
{
    class UnmixItem
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string reconFolder { get; set; }
        public bool isChecked { get; set; }


        public UnmixItem(int _id, string _reconFolder, string _fileName, bool _isChecked)
        {
            id = _id;
            reconFolder = _reconFolder;
            fileName = _fileName;
            isChecked = _isChecked;
        }
    }
}

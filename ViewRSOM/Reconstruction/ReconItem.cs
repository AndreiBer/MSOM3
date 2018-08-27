using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewRSOM.Reconstruction
{
    public class ReconItem
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public bool isChecked { get; set; }

        public ReconItem(int _id, string _fileName, bool _isChecked)
        {
            id = _id;
            fileName = _fileName;
            isChecked = _isChecked;
        }

    }
}

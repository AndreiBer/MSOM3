using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewRSOM.Unmixing
{
    class CompItem
    {
        public int id { get; set; }
        public string compName { get; set; }
        public bool isChecked { get; set; }


        public CompItem(int _id, string _compName, bool _isChecked)
        {
            id = _id;
            compName = _compName;
            isChecked = _isChecked;
        }
    }
}

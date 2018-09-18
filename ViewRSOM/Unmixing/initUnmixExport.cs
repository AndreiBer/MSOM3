using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewRSOM.Unmixing
{
    class initUnmixExport
    {
        public void start(List<UnmixFolderItem> myUnmixFolderItems)
        {
            // open export folder
            List<string> folderNamesExport = new List<string>();
            string folderName;
            // create own private unmix list and setup total progress bar
            for (int i = 0; i < myUnmixFolderItems.Count; i++)
            {
                if (myUnmixFolderItems[i].isChecked)
                {
                    //folderNamesExport.Add(myUnmixFolderItems[i].pathToFOlder + "\\" + myUnmixFolderItems[i].folderName+ "\\Export");
                    folderName = myUnmixFolderItems[i].pathToFOlder + "\\" + myUnmixFolderItems[i].folderName + "\\Export";
                    Process.Start("explorer.exe", @folderName);
                    try
                    {
                        Process.Start(@fileParameters.ImageJ);
                    }
                    catch
                    {
                        Console.WriteLine("ERROR: Cannot open ImageJ. File " + fileParameters.ImageJ + "does not exist. Adapt path in config file.\n");
                    }
                }
                
            }
            Console.WriteLine("Unmix-finished: Export of image stacks is finished");
            Console.WriteLine("All unmixing finished: Export of image stacks is finished"); 
        }
    }
}

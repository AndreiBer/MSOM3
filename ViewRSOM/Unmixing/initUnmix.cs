using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iUnmixing3D;
using iRecon_OpenCl;
using MathWorks.MATLAB.NET.Arrays;
using System.Threading;
using System.IO;
using MathWorks.MATLAB.NET.Utility;



namespace ViewRSOM.Unmixing
{
    class initUnmix
    {
        public void start(List<UnmixItem> _myUnmixItems, List<CompItem> _myCompItems)
        {

            // create timer to estimate recon time
            DateTime tPart = DateTime.Now;
            TimeSpan duration;

            // define own private dataFolder and list of data names that is not updated/affected from outside
            MWArray dataFolder = studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].folderPath + "\\";
            List<string> dataNames = new List<string>();
            List<string> compNames = new List<string>();

            // current unmix settings      
            //unmixingParameters.Spectra = "F:\\MSKCC\\!v2_ViewRSOM_git\\iUnmix\\AllOpticalProperties";
            //unmixingParameters.LaserEnergy = "F:\\MSKCC\\!v2_ViewRSOM_git\\iUnmix\\LaserPowerInnClinic"; ;
            //unmixingParameters.Unmixing3D = 0;

            // create own private unmix list and setup total progress bar
            int N_tot = 0;
            int N_curr = 0;
            for (int i = 0; i < _myUnmixItems.Count; i++)
            {
                if (_myUnmixItems[i].isChecked)
                {
                    dataNames.Add(_myUnmixItems[i].reconFolder + "\\" + _myUnmixItems[i].fileName);
                    N_tot++;
                }
            }
            // create own private unmixComp list
            for (int i = 0; i < _myCompItems.Count; i++)
            {
                if (_myCompItems[i].isChecked)
                {
                    compNames.Add(_myCompItems[i].compName);
                }
            }
            
            //unmixingParameters.unmixProgressTot = new int[2] { 0, N_tot };
            MWCellArray dataName = null;
            MWCellArray compName = null;
            // define data name
            dataName = UnmixfilesToMWarray(dataNames);
            compName = UnmixfilesToMWarray(compNames);
            // Define file parameters
            int counter = 1;
            bool runLoop = true;
            MWArray unmixFolder = null;
            int ind1 = dataNames[0].IndexOf("R_") + 2;
            while (runLoop)
            {
                unmixFolder = dataFolder + "U_" + dataNames[0].Substring(ind1, 7) + counter + "\\";
                if (!Directory.Exists(unmixFolder.ToString()))
                    runLoop = false;
                counter++;
            }
            MWArray unmixLogFolder = unmixFolder + "LogFile\\";
            MWArray unmixImageFolder = unmixFolder + "Images\\";
            MWArray unmixThumbnailFolder = unmixFolder + "Thumbnail\\";
            MWArray unmixExportFolder = unmixFolder + "Export\\";
            MWArray fileExtension = "";
            MWArray Spectra = unmixingParameters.Spectra;
            MWArray LaserEnergy = unmixingParameters.LaserEnergy;
            MWNumericArray Unmixing3D = unmixingParameters.Unmixing3D;

            // create recon folder
            if (!Directory.Exists(unmixFolder.ToString()))
            {
                Directory.CreateDirectory(unmixFolder.ToString());
            }
            if (!Directory.Exists(unmixLogFolder.ToString()))
            {
                Directory.CreateDirectory(unmixLogFolder.ToString());
            }
            if (!Directory.Exists(unmixImageFolder.ToString()))
            {
                Directory.CreateDirectory(unmixImageFolder.ToString());
            }
            if (!Directory.Exists(unmixThumbnailFolder.ToString()))
            {
                Directory.CreateDirectory(unmixThumbnailFolder.ToString());
            }

            // copy file parameters to structure
            string[] fieldNames = { "dataFolder", "unmixingFolder", "unmixingLogFolder", "unmixingImageFolder",
                                          "unmixingThumbnailFolder", "unmixingExportFolder",
                                           "dataName","fileExtension"};
            MWStructArray fP = new MWStructArray(1, 1, fieldNames);
            fP.SetField("dataFolder", dataFolder);
            fP.SetField("unmixingFolder", unmixFolder);
            fP.SetField("unmixingLogFolder", unmixLogFolder);
            fP.SetField("unmixingImageFolder", unmixImageFolder);
            fP.SetField("unmixingThumbnailFolder", unmixThumbnailFolder);
            fP.SetField("unmixingExportFolder", unmixExportFolder);            
            fP.SetField("dataName", dataName);
            fP.SetField("fileExtension", fileExtension);

            //Define unmixing paramteers
            // copy file parameters to structure
            string[] fieldNames2 = { "compName", "Spectra", "LaserEnergy", "Unmixing3D" };
            MWStructArray uP = new MWStructArray(1, 1, fieldNames2);            
            uP.SetField("Spectra", Spectra);
            uP.SetField("LaserEnergy", LaserEnergy);
            uP.SetField("Unmixing3D", Unmixing3D);
            uP.SetField("compName", compName);
            
                iUnmixing3DClass obj = null; // it has to be referenced
                try
                {
                    // Instantiate your component class.                    
                    obj = new iUnmixing3DClass();
                    obj.iUnmixing3D(uP, fP);
                    //N_curr++;
                    //unmixingParameters.unmixProgressTot = new int[2] { N_curr, N_tot };

                    //// Print future time estimate
                    //duration = (DateTime.Now - tPart);
                    //for (int i_time = N_curr + 1; i_time < N_tot; i_time++)
                    //    duration += (DateTime.Now - tPart);
                    //unmixingParameters.remainingUnmixTime = new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);
                    //tPart = DateTime.Now;

                    // Trigger messages in message box
                    Console.WriteLine("Unmix-finished: " + unmixFolder + "U_" + dataName);
                }
                catch (Exception e)
                {
                    //N_curr++;
                    //unmixingParameters.unmixProgressTot = new int[2] { N_curr, N_tot };
                    Console.WriteLine("Status-Unmix: 1.00");
                    if (!e.Message.StartsWith("ERROR:"))
                    {
                        Console.WriteLine("ERROR:" + e.Message + "\n");
                    }
                    if (N_tot == N_curr)
                    {
                        Console.WriteLine("Unmix-finished: unmixing finished with errors.");
                    }
                }
            
            Console.WriteLine("Unmix-finished: All unmixing finished");

        }
        private static MWCellArray UnmixfilesToMWarray(List<string> dataNames)
        {
            MWCellArray unmixFiles = null;
            unmixFiles = new MWCellArray(dataNames.Count);
            MWCharArray dataName = null;
            for (int i_unmix = 0; i_unmix < dataNames.Count; i_unmix++)
            {
                dataName = dataNames[i_unmix];
                unmixFiles[i_unmix + 1] = dataName;
            }
            return unmixFiles;
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;

using iExport;
using MathWorks.MATLAB.NET.Arrays;
using System.Linq;
using System.Diagnostics;

namespace ViewRSOM.Reconstruction
{
    class initExport
    {

        public void start(List<ReconItem> _myReconItems, int N_tot)
        {
            // define own private dataFolder and list of data names that is not updated/affected from outside
            List<string> dataNames = new List<string>();
            List<int> data_iAcq = new List<int>();
            
            // copy recon parameters to structure
            MWNumericArray movingMAP = reconstructionParameters.movingMAP;
            MWNumericArray movingMAP_slabThickness = reconstructionParameters.movingMAP_slabThickness;
            MWNumericArray volume3D = reconstructionParameters.volume3D;
            string[] fieldNames2 = {"movingMAP", "movingMAP_slabThickness", "volume3D" };
            MWStructArray rP = new MWStructArray(1, 1, fieldNames2);
            rP.SetField("movingMAP", movingMAP);
            rP.SetField("movingMAP_slabThickness", movingMAP_slabThickness);
            rP.SetField("volume3D", volume3D);

            // create own private recon list and setup total progress bar
            int N_curr = 0;
            for (int i = 0; i < _myReconItems.Count; i++)
                if (_myReconItems[i].isChecked)
                {
                    dataNames.Add(_myReconItems[i].fileName);
                    data_iAcq.Add(_myReconItems[i].id);
                }
            reconstructionParameters.reconProgressTot = new int[2] { 0, N_tot };

            int N_recon = dataNames.Count;
            int i_acq;

            for (int i = 0; i < N_recon; i++)
            {
                // define data name
                i_acq = data_iAcq[i];

                // run through all subfolders within the current study date
                for (int i_recon = 0; i_recon < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].myReconFolders_list.Count; i_recon++)
                {
                    string reconFolderPath = studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath;
                    string reconFolderWithoutPath = reconFolderPath.Split('\\')[reconFolderPath.Split('\\').Length - 1];

                    // add all reconstruction folders that correspond to the pre-defined naming convention
                    if (reconFolderWithoutPath.Length > 7 && dataNames[i].Length > 5)
                    {
                        if (String.Equals(reconFolderWithoutPath.Substring(0, 8), "R_" + dataNames[i].Substring(0, 6)))
                        {
                            // add recon folder to list
                            string[] reconFiles = Directory.GetFiles(reconFolderPath, "*.mat").ToArray();

                            for (int i_file = 0; i_file < reconFiles.Length; i_file++)
                            {
                                // copy file parameters to structure
                                MWArray dataFile = reconFiles[i_file]; 
                                MWArray reconExportFolder = reconFolderPath + "\\Export\\";
                                MWArray reconLogFolder = reconFolderPath + "\\LogFile\\";
                                string[] fieldNames = { "reconExportFolder", "reconLogFolder", "dataFile" };
                                MWStructArray fP = new MWStructArray(1, 1, fieldNames);
                                fP.SetField("reconExportFolder", reconExportFolder);
                                fP.SetField("reconLogFolder", reconLogFolder);
                                fP.SetField("dataFile", dataFile);

                                iExportClass obj = null;
                                try
                                {
                                    // Instantiate your component class.
                                    obj = new iExportClass();
                                    obj.iExport(fP, rP);
                                    N_curr++;
                                    reconstructionParameters.reconProgressTot = new int[2] { N_curr, N_tot };
                                }
                                catch (Exception e)
                                {
                                    N_curr++;
                                    reconstructionParameters.reconProgressTot = new int[2] { N_curr, N_tot };
                                    // Console.WriteLine("Status-Recon: 1.00");
                                    if (!e.Message.Contains("ERROR:"))
                                    {
                                        Console.WriteLine("ERROR:" + e.Message + "\n");
                                    }
                                    if (N_tot == N_curr)
                                    {
                                        //     Console.WriteLine("Recon-finished: export finished with errors.");
                                    }
                                }

                                // open export folder
                                if (i_file == reconFiles.Length-1)
                                    Process.Start("explorer.exe", @reconExportFolder.ToString());

                            }
                        }
                    }
                }
            }
            try
            {
                Process.Start(@fileParameters.ImageJ);
            }
            catch
            {
                Console.WriteLine("ERROR: Cannot open ImageJ. File " + fileParameters.ImageJ + "does not exist. Adapt path in config file.\n");
            }

            Console.WriteLine("Recon-finished: Export of image stacks is finished");

        }
    }
}

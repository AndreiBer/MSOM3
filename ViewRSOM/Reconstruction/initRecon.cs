using System;
using System.IO;
using System.Collections.Generic;

using iRecon3D;
using iRecon_OpenCl;
using MathWorks.MATLAB.NET.Arrays;
using System.Threading;

namespace ViewRSOM.Reconstruction
{
    class initRecon
    {

        public void start(List<ReconItem> _myReconItems)
        {

            // create timer to estimate recon time
            DateTime tPart = DateTime.Now;
            TimeSpan duration;

            // define own private dataFolder and list of data names that is not updated/affected from outside
            MWArray dataFolder = studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].folderPath + "\\";
            List<string> dataNames = new List<string>();

            // current recon settings
            double zRecon_high_curr = reconstructionParameters.zRecon_high;
            int bandpassSeparation_curr = reconstructionParameters.bandpassSeparation;
            int motionCorrection_curr = reconstructionParameters.motionCorrection;
            int movingMAP_curr = reconstructionParameters.movingMAP;
            int volume3D_curr = reconstructionParameters.volume3D;
            

        // create own private recon list and setup total progress bar
        int N_tot = 0;
            int N_curr = 0;
            for (int i = 0; i < _myReconItems.Count; i++)
                if (_myReconItems[i].isChecked)
                {
                    dataNames.Add(_myReconItems[i].fileName);
                    N_tot++;
                }
            if (bandpassSeparation_curr == 1)
                N_tot = N_tot * 3;
            reconstructionParameters.reconProgressTot = new int[2] { 0, N_tot };

            int N_recon = dataNames.Count;
            MWArray dataName = null;            

            for (int i = 0; i < N_recon; i++)
            {
                // define data name
                dataName = dataNames[i];

                // Define file parameters
                //MWArray dataFolder = studyParameters.studyDateList[studyParameters.studyDate_default].folderPath + "\\";
                int counter = 1;
                bool runLoop = true;
                MWArray reconFolder = null;
                while (runLoop)
                {
                    if (motionCorrection_curr == 1)
                    {
                        reconFolder = dataFolder + "R_" + dataName + "_mc" + counter + "\\";
                    }
                    else
                    {
                        reconFolder = dataFolder + "R_" + dataName + "_" + counter + "\\";
                    }
                    if (!Directory.Exists(reconFolder.ToString()))
                        runLoop = false;
                    counter++;
                }
                MWArray reconLogFolder = reconFolder + "LogFile\\";
                MWArray reconImageFolder = reconFolder + "Images\\";
                MWArray reconThumbnailFolder = reconFolder + "Thumbnail\\";
                MWArray reconExportFolder = reconFolder + "Export\\";
                MWArray SFFile = fileParameters.SFFolder + fileParameters.SFFile;
                MWArray fileExtension = "";

                // create recon folder
                if (!Directory.Exists(reconFolder.ToString()))
                {
                    Directory.CreateDirectory(reconFolder.ToString());
                }
                if (!Directory.Exists(reconLogFolder.ToString()))
                {
                    Directory.CreateDirectory(reconLogFolder.ToString());
                }
                if (!Directory.Exists(reconImageFolder.ToString()))
                {
                    Directory.CreateDirectory(reconImageFolder.ToString());
                }
                if (!Directory.Exists(reconThumbnailFolder.ToString()))
                {
                    Directory.CreateDirectory(reconThumbnailFolder.ToString());
                }

                // copy file parameters to structure
                string[] fieldNames = { "dataFolder", "reconFolder", "reconLogFolder", "reconImageFolder",
                                          "reconThumbnailFolder", "reconExportFolder",
                                          "dataName", "SFFile" , "fileExtension"};
                MWStructArray fP = new MWStructArray(1, 1, fieldNames);
                fP.SetField("dataFolder", dataFolder);
                fP.SetField("reconFolder", reconFolder);
                fP.SetField("reconLogFolder", reconLogFolder);
                fP.SetField("reconImageFolder", reconImageFolder);
                fP.SetField("reconThumbnailFolder", reconThumbnailFolder);
                fP.SetField("reconExportFolder", reconExportFolder);
                fP.SetField("dataName", dataName);
                fP.SetField("SFFile", SFFile);
                fP.SetField("fileExtension", fileExtension);
            
                // Define reconstruction parameters
                MWNumericArray reflectionFilter = reconstructionParameters.reflectionFilter;
                MWNumericArray bandpassFilter = reconstructionParameters.bandpassFilter;
                MWNumericArray f_low = reconstructionParameters.f_low;
                MWNumericArray f_high = reconstructionParameters.f_high;
                MWNumericArray coRegistration = reconstructionParameters.coRegistration;
                MWNumericArray focusModel = reconstructionParameters.focusModel;
                MWNumericArray bpMode = reconstructionParameters.bpMode;
                MWNumericArray bpWeight = reconstructionParameters.bpWeight;
                MWNumericArray v_s = reconstructionParameters.v_s;
                MWNumericArray zRecon_low = reconstructionParameters.zRecon_low; // reconstructionParameters.z_lowRel;
                MWNumericArray zRecon_high = zRecon_high_curr; // reconstructionParameters.z_upRel;
                MWNumericArray motionCorrection = motionCorrection_curr;
                MWNumericArray blockSize = reconstructionParameters.blockSize;
                MWArray platform = reconstructionParameters.platform;
                MWArray deviceType = reconstructionParameters.deviceType;
                MWNumericArray dz = reconstructionParameters.dz;
                MWNumericArray ds = reconstructionParameters.ds;
                MWNumericArray maxOffset = reconstructionParameters.maxOffset;
                MWNumericArray y_shiftInd = reconstructionParameters.y_shiftInd;

                MWNumericArray movingMAP = movingMAP_curr;
                MWNumericArray movingMAP_slabThickness = reconstructionParameters.movingMAP_slabThickness;
                MWNumericArray volume3D = volume3D_curr;
                
                // copy file parameters to structure
                string[] fieldNames2 = { "reflectionFilter", "bandpassFilter", "f_low", "f_high",
                                            "coRegistration", "focusModel", "bpMode", "bpWeight",
                                            "v_s", "zRecon_low", "zRecon_high", "motionCorrection", "blockSize",
                                            "platform", "deviceType", "dz", "ds",
                                            "maxOffset", "y_shiftInd", "movingMAP", "movingMAP_slabThickness", "volume3D"};

                MWStructArray rP = new MWStructArray(1, 1, fieldNames2);
                rP.SetField("reflectionFilter", reflectionFilter);
                rP.SetField("bandpassFilter", bandpassFilter);
                rP.SetField("f_low", f_low);
                rP.SetField("f_high", f_high);
                rP.SetField("coRegistration", coRegistration);
                rP.SetField("focusModel", focusModel);
                rP.SetField("bpMode", bpMode);
                rP.SetField("bpWeight", bpWeight);
                rP.SetField("v_s", v_s);
                rP.SetField("zRecon_low", zRecon_low);
                rP.SetField("zRecon_high", zRecon_high);
                rP.SetField("motionCorrection", motionCorrection);
                rP.SetField("blockSize", blockSize);
                rP.SetField("platform", platform);
                rP.SetField("deviceType", deviceType);
                rP.SetField("dz", dz);
                rP.SetField("ds", ds);
                rP.SetField("maxOffset", maxOffset);
                rP.SetField("y_shiftInd", y_shiftInd);
                rP.SetField("movingMAP", movingMAP);
                rP.SetField("movingMAP_slabThickness", movingMAP_slabThickness);
                rP.SetField("volume3D", volume3D);
                
                // Define reconstruction parameters
                MWNumericArray t_focus = detectorParameters.t_focus;
                MWNumericArray focalLength = detectorParameters.focalLength;
                MWNumericArray detectorDiameter = detectorParameters.detectorDiameter;
                MWNumericArray dataSign = detectorParameters.dataSign;

                // copy file parameters to structure
                string[] fieldNames3 = { "t_focus", "focalLength", "detectorDiameter", "dataSign" };
                MWStructArray dP = new MWStructArray(1, 1, fieldNames3);
                dP.SetField("t_focus", t_focus);
                dP.SetField("focalLength", focalLength);
                dP.SetField("detectorDiameter", detectorDiameter);
                dP.SetField("dataSign", dataSign);

                if (bandpassSeparation_curr == 0)
                {
                    iReconOpenClClass obj = null;
                    // iRecon3DClass obj = null;

                    try
                    {
                        // Instantiate your component class.
                        obj = new iReconOpenClClass();
                        obj.iRecon3D_OpenCl(fP, rP, dP);
                        // obj = new iRecon3DClass();
                        // obj.iRecon3D(fP, rP, dP);
                        N_curr++;
                        reconstructionParameters.reconProgressTot = new int[2] { N_curr, N_tot };

                        // Print future time estimate
                        duration = (DateTime.Now - tPart);
                        for (int i_time = N_curr + 1; i_time < N_tot; i_time++)
                            duration += (DateTime.Now - tPart);
                        reconstructionParameters.remainingReconTime = new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);
                        tPart = DateTime.Now;

                        // Trigger messages in message box
                        Console.WriteLine("Recon-finished: " + reconFolder + "R_" + dataName);
                    }
                    catch (Exception e)
                    {
                        N_curr++;
                        reconstructionParameters.reconProgressTot = new int[2] { N_curr, N_tot };
                        Console.WriteLine("Status-Recon: 1.00");
                        if (!e.Message.StartsWith("ERROR:"))
                        {
                            Console.WriteLine("ERROR:" + e.Message + "\n");
                        }
                        if (N_tot == N_curr)
                        {
                            Console.WriteLine("Recon-finished: reconstruction finished with errors.");
                        }
                    }
                }

                if (bandpassSeparation_curr == 1)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // redefine some parameters
                        if (j == 0)
                        {
                            SFFile = fileParameters.SFFolder + fileParameters.SFFile;
                            fileExtension = "";
                            fP.SetField("SFFile", SFFile);
                            fP.SetField("fileExtension", fileExtension);

                            f_low = reconstructionParameters.f_low;
                            f_high = reconstructionParameters.f_high;
                            rP.SetField("f_low", f_low);
                            rP.SetField("f_high", f_high);
                        }
                        if (j == 1)
                        {
                            SFFile = fileParameters.SFFolder + fileParameters.SFFileLF;
                            fileExtension = "_LF";
                            fP.SetField("SFFile", SFFile);
                            fP.SetField("fileExtension", fileExtension);

                            f_low = reconstructionParameters.f_low;
                            f_high = reconstructionParameters.f_mid;
                            rP.SetField("f_low", f_low);
                            rP.SetField("f_high", f_high);
                        }
                        if (j == 2)
                        {
                            SFFile = fileParameters.SFFolder + fileParameters.SFFileHF;
                            fileExtension = "_HF";
                            fP.SetField("SFFile", SFFile);
                            fP.SetField("fileExtension", fileExtension);

                            f_low = reconstructionParameters.f_mid;
                            f_high = reconstructionParameters.f_high;
                            rP.SetField("f_low", f_low);
                            rP.SetField("f_high", f_high);
                        }

                        iReconOpenClClass obj = null;
                        // iRecon3DClass obj = null;

                        try
                        {
                            // Instantiate your component class.
                            obj = new iReconOpenClClass();
                            obj.iRecon3D_OpenCl(fP, rP, dP);
                            // obj = new iRecon3DClass();
                            // obj.iRecon3D(fP, rP, dP);
                            N_curr++;
                            reconstructionParameters.reconProgressTot = new int[2] { N_curr, N_tot };

                            // Print future time estimate
                            duration = (DateTime.Now - tPart);
                            for (int i_time = N_curr + 1; i_time < N_tot; i_time++)
                                duration += (DateTime.Now - tPart);
                            reconstructionParameters.remainingReconTime = new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);
                            tPart = DateTime.Now;

                            // Trigger messages in message box
                            if (j == 0)
                                Console.WriteLine("Recon-finished: " + reconFolder + "R_" + dataName);
                            else
                                Console.WriteLine("Recon-finished: " + reconFolder + "R_" + dataName + fileExtension);
                        }
                        catch (Exception e)
                        {
                            N_curr++;
                            reconstructionParameters.reconProgressTot = new int[2] { N_curr, N_tot };
                            Console.WriteLine("Status-Recon: 0.01");
                            if (!e.Message.Contains("ERROR:"))
                            {
                                Console.WriteLine("ERROR:" + e.Message + "\n");
                            }
                            if (N_tot == N_curr)
                            {
                                Console.WriteLine("Recon-finished: reconstruction finished with errors.");
                            }

                        }

                    }

                }

            }

            Console.WriteLine("Recon-finished: All reconstructions finished");

        }

    }

}

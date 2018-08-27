using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Input;

namespace ViewRSOM.ConsoleStream
{

    public delegate void CharEventHandler(string sender, string receiver, string value);
    public delegate void AcqStatusEventHandler(string sender, string receiver, string value);
    public delegate void AcqPreviewEventHandler(string sender, string receiver, string value);
    public delegate void QuickScanEventHandler(string sender, string receiver, string value);
    public delegate void ReconStatusEventHandler(string sender, string receiver, string value);
    public delegate void AcqTimeEstimate(string sender, string receiver, string value);
    public delegate void ReconFinishedEventHandler(string sender, string receiver, string value);
    public delegate void OPOWlEventHandler(string sender, string receiver, string value);

    public static class IOEventHandler
    {
        public static event CharEventHandler myIOEvent;
        public static event AcqStatusEventHandler myStatusAcq;
        public static event AcqPreviewEventHandler myPreviewAcq;
        public static event QuickScanEventHandler myQuickScan;
        public static event ReconStatusEventHandler myStatusRecon;
        public static event AcqTimeEstimate myTimeEstimate;
        public static event ReconFinishedEventHandler myReconFinished;
        public static event OPOWlEventHandler myOPOWl;

        // this method can fire an event
        public static void triggerIOEvent(string sender, string receiver, string value)
        {
            // if (myIOEvent != null) myIOEvent(sender, receiver, value);

            // define culture and helping varibale to load the config file
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            NumberStyles styles = NumberStyles.AllowExponent | NumberStyles.Number;

            if (value != null)
            {

                #region error_log
                if (value.Length > 4)
                {
                    if (String.Equals(value.Substring(0, 5), "ERROR"))
                    {
                        // write event log
                        string sSource = "RSOM";
                        string sEvent = value;
                        try
                        {
                            EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 234);
                        }
                        catch
                        {

                        }

                        // write event log to local file
                        try
                        {
                            if (!Directory.Exists(fileParameters.errorLogFolder.ToString()))
                            {
                                Directory.CreateDirectory(fileParameters.errorLogFolder.ToString());
                            }
                            string path = fileParameters.errorLogFolder + System.DateTime.Now.Year.ToString("0000") + System.DateTime.Now.Month.ToString("00") + "_ErrorLog.txt";

                            // Create a file to write to.
                            if (!Directory.Exists(@fileParameters.errorLogFolder))
                                Directory.CreateDirectory(@fileParameters.errorLogFolder);

                            using (StreamWriter sw = File.AppendText(@path))
                            {
                                string date = System.DateTime.Now.Year.ToString("0000") + '-' + System.DateTime.Now.Month.ToString("00") + '-' +
                                    System.DateTime.Now.Day.ToString("00") + '_' + System.DateTime.Now.Hour.ToString("00") + '-' + System.DateTime.Now.Minute.ToString("00") + '-' +
                                    System.DateTime.Now.Second.ToString("00");
                                sw.WriteLine(date + ": " + sEvent);
                            }
                        }
                        catch
                        {
                        }

                        // throw event to write Error in GUI
                        if (myIOEvent != null) myIOEvent(sender, receiver, value);
                        // myIOEvent?.Invoke(sender, receiver, value);
                    }
                }
                #endregion

                #region warning_log
                if (value.Length > 6)
                {
                    if (String.Equals(value.Substring(0, 7), "WARNING"))
                    {
                        // write event log
                        string sSource = "RSOM";
                        string sEvent = value;
                        try
                        {
                            EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Warning, 234);
                        }
                        catch
                        {

                        }

                        // write event log to local file
                        try
                        {
                            if (!Directory.Exists(fileParameters.errorLogFolder.ToString()))
                            {
                                Directory.CreateDirectory(fileParameters.errorLogFolder.ToString());
                            }
                            string path = fileParameters.errorLogFolder + System.DateTime.Now.Year.ToString("0000") + System.DateTime.Now.Month.ToString("00") + "_ErrorLog.txt";

                            // Create a file to write to.
                            if (!Directory.Exists(@fileParameters.errorLogFolder))
                                Directory.CreateDirectory(@fileParameters.errorLogFolder);

                            using (StreamWriter sw = File.AppendText(@path))
                            {
                                string date = System.DateTime.Now.Year.ToString("0000") + '-' + System.DateTime.Now.Month.ToString("00") + '-' +
                                    System.DateTime.Now.Day.ToString("00") + '_' + System.DateTime.Now.Hour.ToString("00") + '-' + System.DateTime.Now.Minute.ToString("00") + '-' +
                                    System.DateTime.Now.Second.ToString("00");
                                sw.WriteLine(date + ": " + sEvent);
                            }
                        }
                        catch
                        {

                        }

                        // throw event to write Error in GUI
                        if (myIOEvent != null) myIOEvent(sender, receiver, value);
                        // myIOEvent?.Invoke(sender, receiver, value);
                    }
                }
                #endregion

                #region other_IO
                if (value.Length > 9)
                {
                    if (String.Equals(value.Substring(0, 10), "Status-Acq"))
                    {
                        if (myStatusAcq != null) myStatusAcq(sender, receiver, value);
                        // myStatusAcq?.Invoke(sender, receiver, value);
                    }
                }

                
                if (value.Length > 13)
                {
                    if (String.Equals(value.Substring(0, 14), "OPO-Wavelength"))
                    {
                        if (myOPOWl != null) myOPOWl(sender, receiver, value);
                        //if (myTimeEstimate != null) myTimeEstimate(sender, receiver, value);
                        // myStatusAcq?.Invoke(sender, receiver, value);
                    }
                }
                 

                if (value.Length > 16)
                {
                    if (String.Equals(value.Substring(0, 17), "Status-InputRange"))
                    {
                        bool canParse;
                        int intParse;
                        int target = 40000;

                        // define optimal input range
                        canParse = Int32.TryParse(value.Substring(18), styles, culture, out intParse);
                        if (canParse)
                        {
                            // calcualte optimal input range
                            double ratio = (double)intParse/(double)target;
                            double optimalIR = acquisitionParameters.inputRange_list[acquisitionParameters.inputRange_list.Count-1] * ratio;

                            acquisitionParameters.inputRange_listIndex = acquisitionParameters.inputRange_list.Count-1;
                            for (int i_IR = acquisitionParameters.inputRange_list.Count-1; i_IR > -1; i_IR--)
                            {
                                if ((double)acquisitionParameters.inputRange_list[i_IR] > optimalIR)
                                {
                                    acquisitionParameters.inputRange_listIndex = i_IR;
                                }
                            }                            
                        }
                    }
                }

                if (value.Length > 10)
                {
                    if (String.Equals(value.Substring(0, 11), "Preview-Acq"))
                    {
                        if (myPreviewAcq != null) myPreviewAcq(sender, receiver, value);
                    }
                }

                if (value.Length > 16)
                {
                    if (String.Equals(value.Substring(0, 17), "Preview-QuickScan"))
                    {
                        if (myQuickScan != null) myQuickScan(sender, receiver, value);
                    }
                }

                if (value.Length > 11)
                {
                    if (String.Equals(value.Substring(0, 12), "Status-Recon"))
                    {
                        if (myStatusRecon != null) myStatusRecon(sender, receiver, value);
                    }
                }

                if (value.Length > 13)
                {
                    if (String.Equals(value.Substring(0, 14), "Recon-finished"))
                    {
                        if (myReconFinished != null) myReconFinished(sender, receiver, value);
                    }
                }

                if (value.Length > 25)
                {
                    if (String.Equals(value.Substring(0, 26), "Estimated measurement time"))
                    {
                        if (myTimeEstimate != null) myTimeEstimate(sender, receiver, value);
                    }
                }
                #endregion
            }

        }
    }
}
